// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Common base class for top-level result objects.
    /// </summary>
    public abstract class ResultBase
    {
        /// <summary>
        /// Gets or sets version of the JSON format.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets tick unit, in Hertz.
        /// </summary>
        public double Timescale { get; set; }

        /// <summary>
        /// Gets or sets video offset, in ticks.
        /// </summary>
        public Int64 Offset { get; set; }

        /// <summary>
        /// Gets or sets rate of frames (frames/second).
        /// </summary>
        public double Framerate { get; set; }

        /// <summary>
        /// Gets or sets width of the frame, in pixels.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets height of frames, in pixels.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets rotation of the video, in degrees clockwise.
        /// </summary>
        public int? Rotation { get; set; }
    }
}
