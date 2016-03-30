//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
//
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/ProjectOxford-ClientSDK
//
// Copyright (c) Microsoft Corporation
// All rights reserved.
//
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace Microsoft.ProjectOxford.Vision
{
    public enum VisualFeature
    {
        /// <summary>
        /// Clipart or a line drawing.
        /// </summary>
        ImageType,

        /// <summary>
        /// Accent color, dominant color, and whether an image is monochromatic.
        /// </summary>
        Color,

        /// <summary>
        /// Faces, if present, coordinates, gender and age.
        /// </summary>
        Faces,

        /// <summary>
        /// Raciness; pornographic in nature (nudity or sex act). Sexually suggestive content is also detected.
        /// </summary>
        Adult,

        /// <summary>
        /// Image categorizations; taxonomy defined in documentation.
        /// </summary>
        Categories,

        /// <summary>
        /// Image tags.
        /// </summary>
        Tags,

        /// <summary>
        /// Image description.
        /// </summary>
        Description
    }
}
