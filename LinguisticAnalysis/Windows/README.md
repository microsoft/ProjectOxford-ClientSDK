Microsoft Cognitive Services Linguistic Analysis API SDK and sample
===============================================================

This repo contains a client library and sample code to demonstrate how to use linguistic analysis APIs.
You can find more information about the APIs in [our documentation](<https://www.microsoft.com/cognitive-services/en-us/linguistic-analysis-api>).

Getting started
===============

To obtain a subscription key for Linguistic Analysis, visit [our website] (<https://www.microsoft.com/cognitive-services/en-us/sign-up>) and sign up for free using your Microsoft account.
Then open, modify, build, and run the accompanying Visual Studio solution.
This sample is a C# Windows console application demonstrating the use of the Microsoft Cognitive Services Linguistic Analysis API.

Build the sample
----------------

1.  Start Microsoft Visual Studio 2015 and select File &gt; Open &gt; Project/Solution.

2.  Starting in the folder where you clone the repository, go to the LinguisticAnalysis &gt; Windows &gt; Sample folder.

3.  Double-click the Visual Studio 2015 Solution file .sln.

4.  Paste your Microsoft Cognitive Services Linguistic Analysis subscription key into the LinguisticsClient constructor parameter value in Program.cs.

5.  Press Ctrl+Shift+B, or select Build &gt; Build Solution.

Run the sample
--------------

After the build is complete, press F5 to run the sample.

A number of lines will be printed on the console, showing the results received from the Linguistics service.
These include: listing the set of available analyzers, and printing the results of those analyzers on some simple text.

Press any key to exit the app.

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
