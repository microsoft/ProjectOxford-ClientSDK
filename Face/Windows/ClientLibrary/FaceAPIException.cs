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

using System;
using System.Net;

namespace Microsoft.ProjectOxford.Face
{
    /// <summary>
    /// Represents client error with detailed error message and error code
    /// </summary>
    public class FaceAPIException : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceAPIException" /> class
        /// </summary>
        public FaceAPIException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceAPIException" /> class
        /// </summary>
        /// <param name="errorCode">Code represents the error category</param>
        /// <param name="errorMessage">Message represents the detailed error description</param>
        /// <param name="statusCode">Http status code</param>
        public FaceAPIException(string errorCode, string errorMessage, HttpStatusCode statusCode)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            HttpStatus = statusCode;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the error code
        /// </summary>
        public string ErrorCode
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets http status of http response.
        /// </summary>
        /// <value>
        /// The HTTP status.
        /// </value>
        public HttpStatusCode HttpStatus
        {
            get; set;
        }

        #endregion Properties
    }
}