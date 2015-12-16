// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// A global object defining a region that will be monitored for motion.  Returned as part of the <see cref="MotionDetectionResult"/> object.
    /// </summary>
    public class MotionRegion
    {
        /// <summary>
        /// Gets or sets id of the region.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets type of region.
        /// Currently only type supported is "rectangle".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets upper-left coordinate of the region, as a fraction of the overall frame width.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets upper-left coordinate of the region, as a fraction of the overall frame height.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets width of the region, as a fraction of the overall frame width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets height of the region, as a fraction of the overall frame height.
        /// </summary>
        public double Height { get; set; }
    }
}
