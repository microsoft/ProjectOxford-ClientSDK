/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System;
    using System.Net;

    /// <summary>
    /// The Exception will be shown to client.
    /// </summary>
    public class ClientException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        public ClientException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        public ClientException(string message)
            : base(message)
        {
            this.Error = new ClientError()
            {
                Code = HttpStatusCode.InternalServerError.ToString(),
                Message = message
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="httpStatus">The Http Status code.</param>
        public ClientException(string message, HttpStatusCode httpStatus)
            : base(message)
        {
            this.HttpStatus = httpStatus;

            this.Error = new ClientError()
            {
                Code = this.HttpStatus.ToString(),
                Message = message
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClientException(string message, Exception innerException)
            : base(message, innerException)
        {
            this.Error = new ClientError()
            {
                Code = HttpStatusCode.InternalServerError.ToString(),
                Message = message
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="httpStatus">The http status.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClientException(string message, string errorCode, HttpStatusCode httpStatus, Exception innerException)
            : base(message, innerException)
        {
            this.HttpStatus = httpStatus;

            this.Error = new ClientError()
            {
                Code = errorCode,
                Message = message
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientException"/> class.
        /// </summary>
        /// <param name="error">The error entity.</param>
        /// <param name="httpStatus">The http status.</param>
        public ClientException(ClientError error, HttpStatusCode httpStatus)
        {
            this.Error = error;
            this.HttpStatus = httpStatus;
        }

        /// <summary>
        /// Gets http status of http response.
        /// </summary>
        /// <value>
        /// The HTTP status.
        /// </value>
        public HttpStatusCode HttpStatus { get; private set; }

        /// <summary>
        /// Gets or sets the httpError message.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public ClientError Error { get; set; }

        /// <summary>
        /// Create Client Exception of Bad Request.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <returns>Client Exception Instance.</returns>
        public static ClientException BadRequest(string message)
        {
            return new ClientException(
                new ClientError()
                {
                    Code = ((int)HttpStatusCode.BadRequest).ToString(),
                    Message = message
                },
                HttpStatusCode.BadRequest);
        }
    }
}
