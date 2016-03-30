The sample
==========

This sample is a set of python scripts to demonstrate the use of Microsoft Cognitive Services (formerly Project Oxford)  Speaker Recognition API.

It demonstrates the speaker identification feature.

Run the sample
--------------

First, you must obtain a Speaker Recognition API subscription key by following instructions in [Microsoft Cognitive Services subscription](<https://www.microsoft.com/cognitive-services/en-us/sign-up>).

To use this sample application, there are four different scenarios:

1. Create a user profile: Execute the following command

           python Identification\CreateProfile.py <subscription_key>

2. Print all user profiles: Execute the following command

           python Identification\PrintAllProfiles.py <subscription_key>

3. Enroll user profiles: Execute the following command

           python Identification\EnrollProfile.py <subscription_key> <profile_id> <enrollment_file_path>

4. Identify test files: Execute the following command

           python Identification\IdentifyFile.py <subscription_key> <identification_file_path> <profile_ids>...

Microsoft will receive the audio files you upload and may use them to improve
Speaker Recognition API and related services. By submitting an audio, you confirm
you have consent from everyone in it.

Contributing
============
We welcome contributions and are always looking for new SDKs, input, and
suggestions. Feel free to file issues on the repo and we'll address them as we can. You can also learn more about how you can help on the [Contribution
Rules & Guidelines](</CONTRIBUTING.md>).

For questions, feedback, or suggestions about Microsoft Cognitive Services, feel free to reach out to us directly.

-   [Cognitive Services UserVoice Forum](<https://cognitive.uservoice.com>)

License
=======

All Microsoft Cognitive Services SDKs and samples are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>).

Sample images are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).
