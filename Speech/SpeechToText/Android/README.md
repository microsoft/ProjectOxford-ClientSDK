The client library
==================

The Speech To Text client library is a client library for Microsoft Cognitive Services (formerly Project Oxford)
Speech To Text APIs.  

The easiest way to consume the client library is to add the com.microsoft.projectoxford:speechrecognition package from Maven Central Repository.

To find the latest version of client library, go to http://search.maven.org, and search for "g:com.microsoft.projectoxford".

To add the client library dependency from build.gradle file, add the following line in dependencies.

```
dependencies {
    //
    // Use the following line to include client library from Maven Central Repository
    // Change the version number from the search.maven.org result
    //
    compile 'com.microsoft.projectoxford:speechrecognition:0.6.0'

    // Your other Dependencies...
}
```

To add the client library dependency from Android Studio:

1. From Menu, Choose File \> Project Structure
2. Click on your app module
3. Click on Dependencies tab
4. Click "+" sign to add new dependency
5. Pick "Library dependency" from the drop down list
6. Type "com.microsoft.projectoxford" and hit the search icon from "Choose Library Dependency" dialog
7. Pick the Project Oxford client library that you intend to use.
8. Click "OK" to add the new dependency
9. Download the appropriate JNI library `libandroid_platform.so` from [this page](https://github.com/Microsoft/ProjectOxford-ClientSDK/tree/master/Speech/SpeechToText/Android/SpeechSDK/libs) and put into your project's directory `app/src/main/jniLibs/armeabi/` or `app/src/main/jniLibs/x86/`.

The Speech Recognition Sample
==========

This sample is an Android application to demonstrate the use of Microsoft Cognitive Services (formerly Project Oxford)
Speech To Text API.

It demonstrates the following features using a wav file or external microphone input:

* Short-form recognition.
* Long-form dictation.
* Recognition with intent.

Requirements
------------

* Android OS must be Android 4.1 or higher (API Level 16 or higher)
* The speech client library contains native code. To use this sample in an emulator, make sure that your build variant matches the architecture (x86 or arm) of your emulator. However, due to the need of audio, using a physical device is recommended.

Build the sample
----------------

1. First, you must obtain a Speech API subscription key by following instructions in [Microsoft Cognitive Services subscription](<https://www.microsoft.com/cognitive-services/en-us/sign-up>).

2.  Start Android Studio and choose "Import project (Eclipse ADT, Gradle, etc.)" from the "Quick Start" options from Speech \> SpeechToText \> Android folder.

3.  A "Gradle Sync" dialog will pop-up, choose OK to continue downloading the latest tools.

4. In Android Studio -\> "Project" panel -\> "Android" view, open file
    "SpeechRecoExample/res/values/strings.xml", and find the line
    "Please\_add\_the\_subscription\_key\_here;". Replace the
    "Please\_add\_the\_subscription\_key\_here" value with your subscription key
    string from the first step. If you cannot find the file "strings.xml", it is
    in folder "Samples\_SpeechRecoExample\_res\_values\_strings.xml".

4.  In Android Studio, select menu "Build \> Make Project" to build the sample,
    and "Run" to launch this sample app.


Running the sample
--------------

In Android Studio, select menu "Run", and "Run app" to launch this sample app.

1.  In the application, press the button "Select Mode" to select what type of Speech would like to use.

2.  For modes where you would like both Speech recognition and Intent to work, you need to sign up [Language Understanding Intelligent Service (LUIS)](<https://www.microsoft.com/cognitive-services/en-us/sign-up>) and set the key values in
    luisAppID and luisSubscriptionID from "Samples\_SpeechRecoExample\_res\_values\_strings.xml".

3. To Start recognition, press the Start button.

<img src="SampleScreenshots/SampleRunning1.png" width="50%"/>

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
