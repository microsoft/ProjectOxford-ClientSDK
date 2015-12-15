The sample
==========

This sample is a Windows WPF application to demonstrate the use of Project
Oxford Speaker Recognition API.

It demonstrates the speaker identification and speaker verification features.

Build the sample
----------------

1.  Start Microsoft Visual Studio 2015 and select File \> Open \>
    Project/Solution.

2.  For speaker identification, starting in the folder where you clone the repository, go to 
    SpeakerRecognition \> Windows \> Identification folder.
    For speaker identification, starting in the folder where you clone the repository, go to 
    SpeakerRecognition \> Windows \> Verification folder.

3.  Double-click the Visual Studio 2015 Solution (.sln) file.

4.  Press Ctrl+Shift+B, or select Build \> Build Solution.

Run the sample
--------------

After the build is complete, press F5 to run the sample.

First, you will need a [Microsoft Azure Account](<http://www.azure.com>) if you don't have one already.

You must obtain a subscription key for Speaker Recognition API by following instructions in [Subscription
key management](<http://www.projectoxford.ai/doc/general/subscription-key-mgmt>).

Locate the text edit box saying "Paste your subscription key here to start". Paste 
your subscription key. You can choose to persist your subscription key in your machine
by clicking "Save Key" button. When you want to delete the subscription key from the
machine, click "Delete Key" to remove it from your machine.

Click on "Select Scenario" to use samples of different scenarios, and
follow the instructions on screen.

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
