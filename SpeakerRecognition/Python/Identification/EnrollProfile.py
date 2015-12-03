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

import IdentificationServiceHttpClientHelper
import sys

def enroll_profiles(subscription_key, enrollment_script_file_path):
    """Creates a profile on the server.

    Arguments:
    subscription_key -- the subscription key string
    enrollment_script_file_path -- the enrollment script file path
    """

    helper = IdentificationServiceHttpClientHelper.IdentificationServiceHttpClientHelper(
        subscription_key)
    enrollment_script_file = open(enrollment_script_file_path)
    print('Profile ID, Total Enrollment Speech Time, Remaining Enrollment Time,'
          ' Speech Time, Enrollment Status')
    for line in enrollment_script_file:
        fields = line.split(',')
        if len(fields) != 2:
            sys.exit('Error: Incorrect script line format: ' + line)
        profile_id = fields[0].strip()
        file_path = fields[1].strip()
        enrollment_response = helper.enroll_profile(profile_id, file_path)
        print('{0}, {1}, {2}, {3}, {4}'.format(
            profile_id,
            enrollment_response.get_total_speech_time(),
            enrollment_response.get_remaining_speech_time(),
            enrollment_response.get_speech_time(),
            enrollment_response.get_enrollment_status()))

if __name__ == "__main__":
    if len(sys.argv) < 3:
        print('Usage: python EnrollProfile.py <subscription_key_file_path> '
              '<enrollment_script_file_path>')
        print('\t<subscription_key_file_path> is the path of a file that'
              ' contains only the subscription key')
        print('\t<enrollment_script_file_path> is the path of a file that'
              ' contains the enrollment script. This should be a csv file where the'
              ' first element is the profile ID and the second element is the enrollment file path')
        sys.exit('Error: Incorrect Usage.')

    SUBSCRIPTION_KEY_FILE = open(sys.argv[1])
    SUBSCRIPTION_KEY = SUBSCRIPTION_KEY_FILE.readline()
    SUBSCRIPTION_KEY_FILE.close()

    enroll_profiles(SUBSCRIPTION_KEY, sys.argv[2])
