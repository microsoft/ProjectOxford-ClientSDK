// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// The operation status.
    /// </summary>
    public enum OperationStatus
    {
        /// <summary>
        /// The not started
        /// </summary>
        NotStarted,

        /// <summary>
        /// The uploading
        /// </summary>
        Uploading,

        /// <summary>
        /// The running
        /// </summary>
        Running,

        /// <summary>
        /// The failed
        /// </summary>
        Failed,

        /// <summary>
        /// The succeeded
        /// </summary>
        Succeeded
    }
}
