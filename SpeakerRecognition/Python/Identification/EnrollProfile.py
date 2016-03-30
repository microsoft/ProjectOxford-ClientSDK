""" Copyright (c) Microsoft. All rights reserved.
Licensed under the MIT license.

Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services

Microsoft Cognitive Services (formerly Project Oxford) GitHub:
https://github.com/Microsoft/ProjectOxford-ClientSDK

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

def enroll_profile(subscription_key, profile_id, file_path):
    """Enrolls a profile on the server.

    Arguments:
    subscription_key -- the subscription key string
    profile_id -- the profile ID of the profile to enroll
    file_path -- the path of the file to use for enrollment
    """
    helper = IdentificationServiceHttpClientHelper.IdentificationServiceHttpClientHelper(
        subscription_key)

    enrollment_response = helper.enroll_profile(profile_id, file_path)

    print('Total Enrollment Speech Time = {0}'.format(enrollment_response.get_total_speech_time()))
    print('Remaining Enrollment Time = {0}'.format(enrollment_response.get_remaining_speech_time()))
    print('Speech Time = {0}'.format(enrollment_response.get_speech_time()))
    print('Enrollment Status = {0}'.format(enrollment_response.get_enrollment_status()))

if __name__ == "__main__":
    if len(sys.argv) < 4:
        print('Usage: python EnrollProfile.py <subscription_key> <profile_id> '
            '<enrollment_file_path>')
        print('\t<subscription_key> is the subscription key for the service')
        print('\t<profile_id> is the profile ID of the profile to enroll')
        print('\t<enrollment_file_path> is the enrollment audio file path')

        sys.exit('Error: Incorrect Usage.')

    enroll_profile(sys.argv[1], sys.argv[2], sys.argv[3])
