// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Top-level object returned by the MotionDetection action.
    /// </summary>
    public class MotionDetectionResult : ResultBase
    {
        /// <summary>
        /// Gets or sets an array of regions in which motions were captured.
        /// </summary>
        public MotionRegion[] Regions { get; set; }

        /// <summary>
        /// Gets or sets an array of fragments containing motion-detection events.
        /// </summary>
        public Fragment<MotionEvent>[] Fragments { get; set; }
    }
}
