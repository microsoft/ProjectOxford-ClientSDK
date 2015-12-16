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

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Microsoft.ProjectOxford.Speech.SpeakerVerification
{
    /// <summary>
    /// A class encapsulating the response returned from the service as a result of a verification request
    /// </summary>
    public class VerificationResult
    {
        /// <summary>
        /// An enum encoding the confidence level of verification
        /// </summary>
        public enum ConfidenceLevel { Low, Normal, High};

        /// <summary>
        /// An enum encoding the speaker verification result
        /// </summary>
        public enum SpeakerVerificationResult { Accept, Reject}

        /// <summary>
        /// The status of the verification result
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SpeakerVerificationResult Result { get; set; }

        /// <summary>
        /// The confidence of the verification result
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ConfidenceLevel Confidence { get; set; }
    }
}
