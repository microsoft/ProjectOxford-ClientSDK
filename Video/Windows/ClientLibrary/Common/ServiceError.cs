// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Common
{
    /// <summary>
    /// Container of ServiceError and Error Entity.
    /// </summary>
    public class ServiceError
    {
        /// <summary>
        /// Gets or sets the ClientError.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public ClientError Error { get; set; }
    }
}
