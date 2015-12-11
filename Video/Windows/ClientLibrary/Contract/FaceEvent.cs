// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// An individual event during FaceDetection action, returned in the <see cref="FaceDetectionResult"/> object.
    /// </summary>
    public class FaceEvent
    {
        /// <summary>
        /// Gets or sets Id of face.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets upper-left coordinate of the face, as a fraction of the overall frame width.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Gets or sets upper-left coordinate of the face, as a fraction of the overall frame height.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Gets or sets width of the face, as a fraction of the overall frame width.
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Gets or sets height of the face, as a fraction of the overall frame height.
        /// </summary>
        public double Height { get; set; }
    }
}
