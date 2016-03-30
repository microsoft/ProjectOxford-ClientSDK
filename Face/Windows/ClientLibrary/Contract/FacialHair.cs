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

namespace Microsoft.ProjectOxford.Face.Contract
{
    /// <summary>
    /// Represents length of moustache, beard and sideburn
    /// </summary>
    public class FacialHair
    {
        #region Properties

        /// <summary>
        /// Gets or sets the moustache value. Represents the length of moustache.
        /// </summary>
        /// <value>
        /// The moustache value.
        /// </value>
        public double Moustache
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the beard value. Represents the length of beard.
        /// </summary>
        /// <value>
        /// The beard value.
        /// </value>
        public double Beard
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the sideburns value. Represents the length of sideburns.
        /// </summary>
        /// <value>
        /// The sideburns value.
        /// </value>
        public double Sideburns
        {
            get; set;
        }

        #endregion Properties
    }
}
