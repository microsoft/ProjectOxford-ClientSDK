Microsoft Project Oxford Web Language Model API SDK and sample
==============================================================

This repo contains a client library and sample code to demonstrate Microsoft’s state-of-the-art language modeling APIs, which automate a variety of standard natural language processing tasks.
Learn more about the Web Language Model API at [our documentation](<https://www.projectoxford.ai/weblm>).

Getting started
===============

First obtain a subscription key for WebLM by following instructions in [Subscription
key management](<http://www.projectoxford.ai/doc/general/subscription-key-mgmt>).
Then open, modify, build, and run the accompanying Visual Studio solution.
This sample is a C# Windows console application demonstrating the use of the Project Oxford Web Language Model API.

Build the sample
----------------

1.  Start Microsoft Visual Studio 2015 and select File \> Open \> Project/Solution.

2.  Starting in the folder where you clone the repository, go to the WebLM \> Windows Folder.

3.  Double-click the Visual Studio 2015 Solution file WebLMSample.sln.

4.  Paste your Oxford subscription key into the LMServiceClient constructor parameter value in Program.cs.

5.  Press Ctrl+Shift+B, or select Build \> Build Solution.

Run the sample
--------------

After the build is complete, press F5 to run the sample.

A number of self-explanatory lines will be printed on the console, showing the results received from the WebLM service.

Press any key to exit the app.

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
