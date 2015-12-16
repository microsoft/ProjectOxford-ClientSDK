// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// The operation type.
    /// </summary>
    public enum OperationType
    {
        /// <summary>
        /// The operation of stabilizing.
        /// </summary>
        Stabilize = 0,

        /// <summary>
        /// The operation of tracking face.
        /// </summary>
        TrackFace = 1,

        /// <summary>
        /// The operation of detacting motion.
        /// </summary>
        DetectMotion = 2
    }
}
