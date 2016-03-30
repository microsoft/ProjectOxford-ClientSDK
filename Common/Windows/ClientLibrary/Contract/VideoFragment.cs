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
    /// Generic class for representing fragments.
    /// A fragment represents a span of time in the video.  Fragments will not overlap in time.
    /// </summary>
    /// <typeparam name="T">Type representing the event type specific to the action.</typeparam>
    public class VideoFragment<T>
    {
        /// <summary>
        /// Gets or sets timestamp of the first event, in ticks.
        /// </summary>
        public Int64 Start { get; set; }

        /// <summary>
        /// Gets or sets overall duration of the fragment, in ticks.
        /// </summary>
        public Int64 Duration { get; set; }

        /// <summary>
        /// Gets or sets the duration of each event, in ticks.  Optional if no events are present.
        /// Note: Duration = Interval * Events.length.
        /// </summary>
        public Int64? Interval { get; set; }

        /// <summary>
        /// Gets or sets array of list of events.  Each array entry is Interval units long in ticks.
        /// </summary>
        public T[][] Events { get; set; }
    }
}
