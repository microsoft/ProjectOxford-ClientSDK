// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Face object returned as part of the <see cref="FaceDetectionResult"/> object.
    /// </summary>
    public class Face
    {
        /// <summary>
        /// Gets or sets Id of face.
        /// </summary>
        public int FaceId { get; set; }
    }
}
