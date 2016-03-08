import http.client
import urllib.parse
import json
import time
from contextlib import closing
import ProfileCreationResponse
import EnrollmentResponse
import VerificationResponse
import VerificationProfile
import logging

class VerificationServiceHttpClientHelper:
    """Abstracts the interaction with the Verification service."""

    _STATUS_OK = 200
    _BASE_URI = 'api.projectoxford.ai'
    _VERIFICATION_PROFILES_URI = '/spid/v1.0/verificationProfiles'
    _VERIFICATION_URI = '/spid/v1.0/verify'
    _SUBSCRIPTION_KEY_HEADER = 'Ocp-Apim-Subscription-Key'
    _CONTENT_TYPE_HEADER = 'Content-Type'
    _JSON_CONTENT_HEADER_VALUE = 'application/json'
    _STREAM_CONTENT_HEADER_VALUE = 'application/octet-stream'

    def __init__(self, subscription_key):
        """Constructor of the VerificationServiceHttpClientHelper class.

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
                self._VERIFICATION_PROFILES_URI,
                self._JSON_CONTENT_HEADER_VALUE)

            if res.status == self._STATUS_OK:
                # Parse the response body
                profiles_raw = json.loads(message)
                return [VerificationProfile.VerificationProfile(profiles_raw[i])
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
                self._VERIFICATION_PROFILES_URI,
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
                self._VERIFICATION_PROFILES_URI,
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
            else:
                raise Exception('Error enrolling profile: ' + res.reason)
        except:
            logging.error('Error enrolling profile.')
            raise

    def verify_file(self, file_path, profile_id):
        """Verifies a profile using an audio file and returns a

        Arguments:
        file_path -- the file path of the audio file to test
        profile_id -- a profile to test against
        """
        try:
            # Prepare the request
            request_url = '{0}?verificationProfileId={1}'.format(
                self._VERIFICATION_URI,
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
                return VerificationResponse.VerificationResponse(json.loads(message))
            else:
                raise Exception('Error verifying audio from file: ' + res.reason)
        except:
            logging.error('Error performing verification.')
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
