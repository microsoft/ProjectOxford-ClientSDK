Microsoft Project Oxford Speech-to-Text API client SDK and samples
==================================================

This repo contains the client libraries that demonstrate Microsoft’s algorithms
to process spoken language. With these APIs, developers can easily include the
ability to add speech driven actions to their applications. In certain cases,
the APIs also allow for real-time interaction with the user as well. See the
tech in action on [our demo page](<https://www.projectoxford.ai/demo/speech>) or
learn more about the API with [our
documentation](<https://www.projectoxford.ai/doc/speech/overview>).

### Speech recognition

Convert spoken audio to text. The API can be directed to turn on and recognize
audio coming from the microphone in real-time, recognize audio coming from a
different real-time audio source, or to recognize audio from within a file. In
all cases, real-time streaming is available, so as the audio is being sent to
the server, partial recognition results are also being returned.

### Speech intent recognition

Convert spoken audio to intent. Similar to Speech Recognition, Speech Intent
Recognition -in addition to returning recognized text from audio input- returns
structured information about the incoming speech so that apps can easily parse
the intent of the speaker, and subsequently drive further action.

Getting started
===============

To get started, select the platform for which you're developing.

-   [Android](</Speech/SpeechToText/Android/)

-   [Windows](</Speech/SpeechToText/Windows/>)

Contributing
============
We welcome contributions and are always looking for new SDKs, input, and
suggestions. Feel free to file issues on the repo and we'll address them as we can. You can also learn more about how you can help on the [Contribution
Rules & Guidelines](</CONTRIBUTING.md>).

For questions, feedback, or suggestions about Project Oxford services, feel free to reach out to us directly.

-   [Project Oxford support](<mailto:oxfordSup@microsoft.com?subject=Project%20Oxford%20Support>)

-   [Forums](<https://social.msdn.microsoft.com/forums/azure/en-US/home?forum=mlapi>)

-   [Blog](<https://blogs.technet.com/b/machinelearning/archive/tags/project+oxford/default.aspx>)

License
=======

All Project Oxford SDKs and samples are licensed with the MIT License. For more details, see
[LICENSE](</LICENSE.md>).

Sample images are licensed separately, please refer to [LICENSE-IMAGE](</LICENSE-IMAGE.md>).
