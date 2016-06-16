Microsoft Cognitive Services Face client SDK and samples
========================================

This repo contains the client libraries that demonstrate Microsoft’s cloud-based
face algorithms to detect and recognize human faces in images. See the tech in
action on [our demo page](<http://www.microsoft.com/cognitive-services/en-us/face-api>) or
learn more about the API with [our
documentation](<http://www.microsoft.com/cognitive-services/en-us/face-api/documentation/overview>).

### Face detection with attribute extraction

Detect human faces in image with face rectangles and face attributes including
face landmarks, pose, gender and age

### Face verification

Check if two faces belonging to same person or not, with confidence score

### Face Grouping

Organize many faces into face groups based on their visual similarity

### Face identification

Search which specific person entity a query face belongs to, from user-provided
person-face data

Getting started
===============

To get started, select the platform for which you're developing.

-   [Android](</Face/Android/>)
-   [Python](</Face/Python/>)
-   [Windows](</Face/Windows/>)
-   [iOS] (</Face/iOS/>)

Changes
============
This document is targeting Microsoft Cognitive Services V1.0 service. For user who has experiences on using Microsoft Cognitive Services (formerly Project Oxford) Face V0, there are some major changes we would like you to know before switching from Project Oxford Face V0 to Microsoft Cognitive Services V1.0 service.

-   **API Signature**

    In Microsoft Cognitive Services (formerly Project Oxford) V1.0, Service root endpoint changes from [https://api.projectoxford.ai/face/v0/]() to [https://api.projectoxford.ai/face/v1.0/]()
There are several signature changes for API, such as [Face - Detect](https://dev.projectoxford.ai/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395236), [Face - Identify](https://dev.projectoxford.ai/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395239), [Face - Find Similar](https://dev.projectoxford.ai/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395237), [Face - Group](https://dev.projectoxford.ai/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395238).

-   **Persisted Data**

    Existing Person Group and Person data which has been setup with Project Oxford Face V0 cannot be accessed with Microsoft Cognitive Services V1.0 service. This incompatible issue will occur for only this one time, following API updates will not affect persisted data any more.

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
