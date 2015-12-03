To use this sample application, there are four different scenarios:

1- Create a user profile
2- Print all user profiles
3- Enroll user profiles
4- Identify test files

1 - Create a user profile :
===========================
    1- Create a text file that contains only the subscription key
    2- Execute the following command
           python CreateProfile.py <subscription_key_file_path>

2 - Print all user profiles :
=============================
    1- Create a text file that contains only the subscription key
    2- Execute the following command
           python PrintAllProfiles.py <subscription_key_file_path>

3 - Enroll user profiles :
==========================
    1- Create a text file that contains only the subscription key
    2- Create a csv file that has the first field as the profile ID and the second field as the 
       enrollment audio file path. You can add more lines to enroll more users in a single command.
    3- Execute the following command
           python EnrollProfile.py <subscription_key_file_path> <enrollment_script_file_path>

3 - Identify test files :
=========================
    1- Create a text file that contains only the subscription key
    2- Create a csv file that has the first field as the test file path and the remaining elements
       are the test profile IDs. You can add more lines to test more files in a single command.
    3- Execute the following command
           python IdentifyFile.py <subscription_key_file_path> <identify_script_file_path>
