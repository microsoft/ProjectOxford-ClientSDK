// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// The operation result class.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public OperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the created date time.
        /// </summary>
        /// <value>
        /// The created date time.
        /// </value>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the last action date time.
        /// </summary>
        /// <value>
        /// The last action date time.
        /// </value>
        public DateTime LastActionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        /// <value>
        /// The resource location.
        /// </value>
        public string ResourceLocation { get; set; }

        /// <summary>
        /// Gets or sets the processing result.
        /// </summary>
        /// <value>
        /// The processing result.
        /// </value>
        public string ProcessingResult { get; set; }
    }
}
