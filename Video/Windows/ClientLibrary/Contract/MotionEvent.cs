// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// An individual event during MotionDetection action, returned in the <see cref="MotionDetectionResult"/> object.
    /// </summary>
    public class MotionEvent
    {
        /// <summary>
        /// Gets or sets id of the motion type.
        /// Currently the value will be 2, indicating that this is a 'motion of interest.'
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// Gets or sets id of the region. 
        /// </summary>
        public int RegionId { get; set; }
    }
}
