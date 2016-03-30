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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.ProjectOxford.Face.Contract
{
    /// <summary>
    /// The face attributes class that holds Age/Gender/Head Pose/Smile/Facial Hair information.
    /// </summary>
    public class FaceAttributes
    {
        #region Properties

        /// <summary>
        /// Gets or sets the age value.
        /// </summary>
        /// <value>
        /// The age value.
        /// </value>
        public double Age
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public string Gender
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the head pose.
        /// </summary>
        /// <value>
        /// The head pose.
        /// </value>
        public HeadPose HeadPose
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the smile value. Represents the confidence of face is smiling.
        /// </summary>
        /// <value>
        /// The smile value.
        /// </value>
        public double Smile
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the facial hair.
        /// </summary>
        /// <value>
        /// The facial hair.
        /// </value>
        public FacialHair FacialHair
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the glasses type.
        /// </summary>
        /// <value>
        /// The glasses type.
        /// </value>
        [JsonConverter(typeof(StringEnumConverter))]
        public Glasses Glasses
        {
            get; set;
        }

        #endregion Properties
    }
}
