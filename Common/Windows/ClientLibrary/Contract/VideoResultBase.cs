// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

using System;

namespace Microsoft.ProjectOxford.Common.Contract
{
    /// <summary>
    /// Common base class for top-level video result objects.
    /// </summary>
    public class VideoResultBase
    {
        /// <summary>
        /// Gets or sets version of the JSON format.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets tick unit, in Hertz.
        /// </summary>
        public double Timescale { get; set; }

        /// <summary>
        /// Gets or sets video offset, in ticks.
        /// </summary>
        public Int64 Offset { get; set; }

        /// <summary>
        /// Gets or sets rate of frames (frames/second).
        /// </summary>
        public double Framerate { get; set; }

        /// <summary>
        /// Gets or sets width of the frame, in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets height of frames, in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets rotation of the video, in degrees clockwise.
        /// </summary>
        public int? Rotation { get; set; }
    }
}
