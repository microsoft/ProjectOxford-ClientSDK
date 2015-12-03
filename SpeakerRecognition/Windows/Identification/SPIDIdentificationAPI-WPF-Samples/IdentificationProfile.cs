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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIDIdentificationAPI_WPF_Samples
{
    /// <summary>
    /// This class represents the response returning from the service
    /// </summary>
    public class IdentificationProfile
    {
        /// <summary>
        /// Speaker profile ID
        /// </summary>
        public string IdentificationProfileId { get; set; }

        /// <summary>
        /// User profile locale
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// The total length of audio submitted for enrollment
        /// </summary>
        public string EnrollmentSpeechTime { get; set; }

        /// <summary>
        /// The remaining audio length for the user to be enrolled
        /// </summary>
        public string RemainingEnrollmentSpeechTime { get; set; }

        /// <summary>
        /// User profile creation date and time
        /// </summary>
        public string CreatedDateTime { get; set; }

        /// <summary>
        /// The date and time of the last action performed on this user profile
        /// </summary>
        public string LastActionDateTime { get; set; }

        /// <summary>
        /// The enrollment status of the profile
        /// </summary>
        public string EnrollmentStatus { get; set; }
    }
}
