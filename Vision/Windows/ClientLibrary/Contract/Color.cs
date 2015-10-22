//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK Github:
// https://github.com/Microsoft/ProjectOxfordSDK-Windows
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

namespace Microsoft.ProjectOxford.Vision.Contract
{

    /// <summary>
    /// The class for color.
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Gets or sets the color of the accent.
        /// </summary>
        /// <value>
        /// The color of the accent.
        /// </value>
        public string AccentColor { get; set; }

        /// <summary>
        /// Gets or sets the dominant color foreground.
        /// </summary>
        /// <value>
        /// The dominant color foreground.
        /// </value>
        public string DominantColorForeground { get; set; }

        /// <summary>
        /// Gets or sets the dominant color background.
        /// </summary>
        /// <value>
        /// The dominant color background.
        /// </value>
        public string DominantColorBackground { get; set; }

        /// <summary>
        /// Gets or sets the dominant colors.
        /// </summary>
        /// <value>
        /// The dominant colors.
        /// </value>
        public string[] DominantColors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is binary image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is binary image; otherwise, <c>false</c>.
        /// </value>
        public bool IsBWImg { get; set; }
    }
}
