// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Project Oxford: http://ProjectOxford.ai
// 
// Project Oxford SDK Github:
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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.ProjectOxford.Speech.SpeakerIdentification
{
    /// <summary>
    /// Encapsulates the enrollment response for a profile
    /// </summary>
    public class EnrollmentResponse
    {
        /// <summary>
        /// The enrollment status of the profile
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public EnrollmentStatus EnrollmentStatus { get; set; }

        /// <summary>
        /// The remaining enrollment speech time
        /// </summary>
        public double RemainingEnrollmentSpeechTime { get; set; }

        /// <summary>
        /// The total speech time in the submitted enrollment
        /// </summary>
        public double SpeechTime { get; set; }

        /// <summary>
        /// The total enrollment speech time submitted for this profile (includes previous enrollment files sent)
        /// </summary>
        public double EnrollmentSpeechTime { get; set; }
    }
}
