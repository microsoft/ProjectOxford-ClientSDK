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
using System.Runtime.Serialization;

namespace Microsoft.ProjectOxford.Face.Contract
{
    /// <summary>
    /// Represents client error with detailed error message and error code
    /// </summary>
    [DataContract]
    public class ClientError
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientError" /> class
        /// </summary>
        public ClientError()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the detailed error message and error code
        /// </summary>
        [DataMember(Name = "error")]
        public ClientExceptionMessage Error
        {
            get;
            set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Represents detailed error message and error code
    /// </summary>
    [DataContract]
    public class ClientExceptionMessage
    {
        #region Properties

        /// <summary>
        /// Gets or sets the detailed error code
        /// </summary>
        [DataMember(Name = "code")]
        public string ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the detailed error message
        /// </summary>
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }

        #endregion Properties
    }

    /// <summary>
    /// Represents client error with detailed error message and error code
    /// </summary>
    [DataContract]
    public class ServiceError
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceError" /> class
        /// </summary>
        public ServiceError()
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the detailed error message and error code
        /// </summary>
        [DataMember(Name = "statusCode")]
        public string ErrorCode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the detailed error message and error code
        /// </summary>
        [DataMember(Name = "message")]
        public string Message
        {
            get;
            set;
        }

        #endregion Properties
    }
}