# The Client Library

The Speech To Text API client library is a thin C\# client wrapper for [Project Oxford Speech To Text](https://www.projectoxford.ai/speech) REST APIs.  

## Installation

To install, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

For x86:

```
PM> Install-Package Microsoft.ProjectOxford.SpeechRecognition-x86
```

For x64: 

```
PM> Install-Package Microsoft.ProjectOxford.SpeechRecognition-x64
```

# Sample App

The sample app is a Windows WPF application that demonstrates the Project Oxford Speech to Text API. 
Using either a wav file or external microphone input, the sample app demonstrates:

* Short-form recognition.
* Long-form dictation.
* Recognition with intent.

## Building the Sample App

1.  Open the `Speech\SpeechToText\Windows\SpeechToText-WPF-Sample.sln` in Visual Studio 2015. 
2.  Choose the build flavor to be x64. This is important because the sample is using Microsoft.ProjectOxford.SpeechRecognition-x64 nuget package by default.
3.  Press Ctrl+Shift+B, or select Build \> Build Solution.

To enable intent recogition, you need to sign up [Language Understanding Intelligent Service (LUIS)](<https://www.projectoxford.ai/luis>) and update `app.config` with related LUIS keys. 

```
  <appSettings>
    <add key="luisAppID" value="<YOUR-LUIS-APP-ID-HERE>" />
    <add key="luisSubscriptionID" value="<YOUR-LUIS-SUBSCRIPTION-ID-HERE" />
  </appSettings>
```

## Running the Sample App

After the build is complete, press F5 to run the sample.

<img src="SampleScreenshots/SampleRunning1.png" width="100%"/>

First, you will need a [Microsoft Azure Account](<http://www.azure.com>) if you don't have one already.
You must obtain a subscription key for Speech API by following instructions in [Subscription Key Management](<http://www.projectoxford.ai/doc/general/subscription-key-mgmt>).

Locate the text edit box saying "Paste your subscription key here to start" on
the top right corner. Paste your subscription key. You can choose to persist
your subscription key in your machine by clicking "Save Key" button. When you
want to delete the subscription key from the machine, click "Delete Key" to
remove it from your machine.

Microsoft will receive the audio you upload and may use them to improve Speech
API and related services. By submitting an audio, you confirm you have consent
from everyone in it.

# Contributing

We welcome contributions and are always looking for new SDKs, input, and suggestions. Learn more about how you can help by reading the [Contribution
Rules & Guidelines](</CONTRIBUTING.md>).

Additionally, feel free to reach out to us directly with questions, feedback, or suggestions.

-   [Project Oxford support](<mailto:oxfordSignup@microsoft.com?subject=Project%20Oxford%20Sign%20Up>)
-   [Forums](<https://social.msdn.microsoft.com/forums/azure/en-US/home?forum=mlapi>)
-   [Blog](<https://blogs.technet.com/b/machinelearning/archive/tags/project+oxford/default.aspx>)

# License

All Project Oxford SDKs are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>), included in both the repo and specific SDK roots.

Sample images used in SDK are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).
