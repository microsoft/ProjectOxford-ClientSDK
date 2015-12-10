""" Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license.

Project Oxford: http://ProjectOxford.ai

Project Oxford SDK Github:
https://github.com/Microsoft/ProjectOxfordSDK-Windows

Copyright (c) Microsoft Corporation
All rights reserved.

MIT License:
Permission is hereby granted, free of charge, to any person obtaining
a copy of this software and associated documentation files (the
"Software"), to deal in the Software without restriction, including
without limitation the rights to use, copy, modify, merge, publish,
distribute, sublicense, and/or sell copies of the Software, and to
permit persons to whom the Software is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
"""

import http.client
import urllib.parse
import json
import time
from contextlib import closing
import IdentificationProfile
import IdentificationResponse
import EnrollmentResponse
import ProfileCreationResponse
import logging

class IdentificationServiceHttpClientHelper:
    """Abstracts the interaction with the Identification service."""

    _STATUS_OK = 200
    _STATUS_ACCEPTED = 202
    _BASE_URI = 'api.projectoxford.ai'
    _IDENTIFICATION_PROFILES_URI = '/spid/v1.0/identificationProfiles'
    _IDENTIFICATION_URI = '/spid/v1.0/identify'
    _SUBSCRIPTION_KEY_HEADER = 'Ocp-Apim-Subscription-Key'
    _CONTENT_TYPE_HEADER = 'Content-Type'
    _JSON_CONTENT_HEADER_VALUE = 'application/json'
    _STREAM_CONTENT_HEADER_VALUE = 'application/octet-stream'
    _OPERATION_LOCATION_HEADER = 'Operation-Location'
    _OPERATION_STATUS_FIELD_NAME = 'status'
    _OPERATION_PROC_RES_FIELD_NAME = 'processingResult'
    _OPERATION_MESSAGE_FIELD_NAME = 'message'
    _OPERATION_STATUS_SUCCEEDED = 'succeeded'
    _OPERATION_STATUS_FAILED = 'failed'
    _OPERATION_STATUS_UPDATE_DELAY = 5

    def __init__(self, subscription_key):
        """Constructor of the IdentificationServiceHttpClientHelper class.

        Arguments:
        subscription_key -- the subscription key string
        """
        self._subscription_key = subscription_key

    def get_all_profiles(self):
        """Return a list of all profiles on the server."""
        try:
            # Send the request
            res, message = self._send_request(
                'GET',
                self._BASE_URI,
                self._IDENTIFICATION_PROFILES_URI,
                self._JSON_CONTENT_HEADER_VALUE)

            if res.status == self._STATUS_OK:
                # Parse the response body
                profiles_raw = json.loads(message)
                return [IdentificationProfile.IdentificationProfile(profiles_raw[i])
                        for i in range(0, len(profiles_raw))]
            else:
                raise Exception('Error getting all profiles: ' + res.reason)
        except:
            logging.error('Error getting all profiles.')
            raise

    def create_profile(self, locale):
        """Creates a profile on the server and returns a dictionary of the creation response.

        Arguments:
        locale -- the locale string for the profile
        """
        try:
            # Prepare the body of the message
            body = json.dumps({'locale': '{0}'.format(locale)})

            # Send the request
            res, message = self._send_request(
                'POST',
                self._BASE_URI,
                self._IDENTIFICATION_PROFILES_URI,
                self._JSON_CONTENT_HEADER_VALUE,
                body)

            if res.status == self._STATUS_OK:
                # Parse the response body
                return ProfileCreationResponse.ProfileCreationResponse(json.loads(message))
            else:
                message = res.read().decode('utf-8')
                raise Exception('Error creating profile: ' + res.reason)
        except:
            logging.error('Error creating profile.')
            raise

    def enroll_profile(self, profile_id, file_path):
        """Enrolls a profile using an audio file and returns a
        dictionary of the enrollment response.

        Arguments:
        profile_id -- the profile ID string of the user to enroll
        file_path -- the file path string of the audio file to use
        """
        try:
            # Prepare the request
            request_url = '{0}/{1}/enroll'.format(
                self._IDENTIFICATION_PROFILES_URI,
                urllib.parse.quote(profile_id))

            # Prepare the body of the message
            with open(file_path, 'rb') as body:
                # Send the request
                res, message = self._send_request(
                    'POST',
                    self._BASE_URI,
                    request_url,
                    self._STREAM_CONTENT_HEADER_VALUE,
                    body)

            if res.status == self._STATUS_OK:
                # Parse the response body
                return EnrollmentResponse.EnrollmentResponse(json.loads(message))
            elif res.status == self._STATUS_ACCEPTED:
                operation_url = res.getheader(self._OPERATION_LOCATION_HEADER)

                return EnrollmentResponse.EnrollmentResponse(
                    self._poll_operation(operation_url))
            else:
                raise Exception('Error enrolling profile: ' + res.reason)
        except:
            logging.error('Error enrolling profile.')
            raise

    def identify_file(self, file_path, test_profile_ids):
        """Enrolls a profile using an audio file and returns a
        dictionary of the enrollment response.

        Arguments:
        file_path -- the file path of the audio file to test
        test_profile_ids -- an array of test profile IDs strings
        """
        try:
            # Prepare the request
            if len(test_profile_ids) < 1:
                raise Exception('Error identifying file: no test profile IDs are provided.')
            test_profile_ids_str = ','.join(test_profile_ids)
            request_url = '{0}?identificationProfileIds={1}'.format(
                self._IDENTIFICATION_URI,
                urllib.parse.quote(test_profile_ids_str))

            # Prepare the body of the message
            with open(file_path, 'rb') as body:
                # Send the request
                res, message = self._send_request(
                    'POST',
                    self._BASE_URI,
                    request_url,
                    self._STREAM_CONTENT_HEADER_VALUE,
                    body)

            if res.status == self._STATUS_OK:
                # Parse the response body
                return IdentificationResponse.IdentificationResponse(json.loads(message))
            elif res.status == self._STATUS_ACCEPTED:
                operation_url = res.getheader(self._OPERATION_LOCATION_HEADER)

                return IdentificationResponse.IdentificationResponse(
                    self._poll_operation(operation_url))
            else:
                raise Exception('Error identifying file: ' + res.reason)
        except:
            logging.error('Error identifying file.')
            raise

    def _poll_operation(self, operation_url):
        """Polls on an operation till it is done

        Arguments:
        operation_url -- the url to poll for the operation status
        """
        try:
            # Parse the operation URL
            parsed_url = urllib.parse.urlparse(operation_url)

            while True:
                # Send the request
                res, message = self._send_request(
                    'GET',
                    parsed_url.netloc,
                    parsed_url.path,
                    self._JSON_CONTENT_HEADER_VALUE)

                if res.status != self._STATUS_OK:
                    raise Exception('Operation Error: ' + res.reason)

                # Parse the response body
                operation_response = json.loads(message)

                if operation_response[self._OPERATION_STATUS_FIELD_NAME] == \
                        self._OPERATION_STATUS_SUCCEEDED:
                    return operation_response[self._OPERATION_PROC_RES_FIELD_NAME]
                elif operation_response[self._OPERATION_STATUS_FIELD_NAME] == \
                        self._OPERATION_STATUS_FAILED:
                    raise Exception('Operation Error: ' +
                                    operation_response[self._OPERATION_MESSAGE_FIELD_NAME])
                else:
                    time.sleep(self._OPERATION_STATUS_UPDATE_DELAY)
        except:
            logging.error('Error polling the operation status.')
            raise

    def _send_request(self, method, base_url, request_url, content_type_value, body=None):
        """Sends the request to the server then returns the response and the response body string.

        Arguments:
        method -- specifies whether the request is a GET or POST request
        base_url -- the base url for the connection
        request_url -- the request url for the connection
        content_type_value -- the value of the content type field in the headers
        body -- the body of the request (needed only in POST methods)
        """
        try:
            # Set the headers
            headers = {self._CONTENT_TYPE_HEADER: content_type_value,
                       self._SUBSCRIPTION_KEY_HEADER: self._subscription_key}

            # Start the connection
            with closing(http.client.HTTPSConnection(base_url)) as conn:
                # Send the request
                conn.request(method, request_url, body, headers)
                res = conn.getresponse()
                message = res.read().decode('utf-8')

                return res, message
        except:
            logging.error('Error sending the request.')
            raise
