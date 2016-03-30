The client library
==================

The Speech To Text API client library is a thin C\# client wrapper for Microsoft Cognitive Services (formerly Project Oxford) Speech To Text
REST APIs.  

The easiest way to use this client library is to get microsoft.projectoxford.vision package from [nuget](<http://nuget.org>).

There are two nuget packages. One is for x86 build, and one is for x64 build.

For x86 package, please go to [Speech Recognition API x86 Package in nuget](https://www.nuget.org/packages/Microsoft.ProjectOxford.SpeechRecognition-x86/) for more details.

For x64 package, please go to [Speech Recognition API x64 Package in nuget](https://www.nuget.org/packages/Microsoft.ProjectOxford.SpeechRecognition-x64/) for more details.


The sample
==========

This sample is a Windows WPF application to demonstrate the use of Microsoft Cognitive Services (formerly Project Oxford) Speech To Text API.

It demonstrates the following features using a wav file or external microphone input:

* Short-form recognition.
* Long-form dictation.
* Recognition with intent.

Build the sample
----------------

1.  Start Microsoft Visual Studio 2015 and select File \> Open \>
    Project/Solution.

2.  Starting in the folder where you clone the repository, go to Speech \> SpeechToText \> Windows Folder.

3.  Double-click the Visual Studio 2015 Solution (.sln) file
    SpeechToText-WPF-Sample.

4. Choose the build flavor to be x64. This is important because the sample is using Microsoft.ProjectOxford.SpeechRecognition-x64 nuget package by default.

5.  Press Ctrl+Shift+B, or select Build \> Build Solution.

For intent recognition to work, you need to sign up [Language Understanding Intelligent Service (LUIS)](<https://www.microsoft.com/cognitive-services/en-us/sign-up>). Please put your LUIS App ID and Subscription ID in app.config file. app.config file can be located from Solution Explorer.

<img src="SampleScreenshots/SampleRunning1.png" width="100%"/>

Run the sample
--------------

After the build is complete, press F5 to run the sample.

First, you must obtain a Speech API subscription key by following instructions in [Microsoft Cognitive Services subscription](<https://www.microsoft.com/cognitive-services/en-us/sign-up>).

Locate the text edit box saying "Paste your subscription key here to start" on
the top right corner. Paste your subscription key. You can choose to persist
your subscription key in your machine by clicking "Save Key" button. When you
want to delete the subscription key from the machine, click "Delete Key" to
remove it from your machine.

Microsoft will receive the audio you upload and may use them to improve Speech
API and related services. By submitting an audio, you confirm you have consent
from everyone in it.

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
