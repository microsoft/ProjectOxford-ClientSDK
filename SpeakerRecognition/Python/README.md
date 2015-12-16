The sample
==========

This sample is a set of python scripts to demonstrate the use of Project
Oxford Speaker Recognition API.

It demonstrates the speaker identification feature.

Run the sample
--------------

First, you will need a [Microsoft Azure Account](<http://www.azure.com>) if you don't have one already.

You must obtain a subscription key for Speaker Recognition API by following instructions in [Subscription
key management](<http://www.projectoxford.ai/doc/general/subscription-key-mgmt>).

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
Rules & Guidelines](<CONTRIBUTING.md>).

For questions, feedback, or suggestions about Project Oxford services, feel free to reach out to us directly.

-   [Project Oxford support](<mailto:oxfordSup@microsoft.com?subject=Project%20Oxford%20Support>)

-   [Forums](<https://social.msdn.microsoft.com/forums/azure/en-US/home?forum=mlapi>)

-   [Blog](<https://blogs.technet.com/b/machinelearning/archive/tags/project+oxford/default.aspx>)

License
=======

All Project Oxford SDKs and samples are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>).

Sample images are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).
