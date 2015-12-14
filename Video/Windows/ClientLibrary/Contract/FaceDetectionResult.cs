// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Top-level object returned by the FaceDetection action.
    /// </summary>
    public class FaceDetectionResult : ResultBase
    {
        /// <summary>
        /// Gets or set an array of faces detected.
        /// </summary>
        public Face[] FacesDetected { get; set; }

        /// <summary>
        /// Gets or sets an array of fragments containing face-detection events.
        /// </summary>
        public Fragment<FaceEvent>[] Fragments { get; set; }
    }
}
