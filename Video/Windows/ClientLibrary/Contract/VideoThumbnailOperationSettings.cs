// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Settings for video thumbnail operation.
    /// </summary>
    public class VideoThumbnailOperationSettings : VideoOperationSettings
    {
        /// <summary>
        /// Output type of thumbnail result.
        /// </summary>
        public OutputTypes OutputType { get; set; }

        /// <summary>
        /// Indicates whether output video should include audio track.
        /// </summary>
        public bool OutputAudio { get; set; }

        /// <summary>
        /// Indicates whether output video should have fade in/out when scene changes.
        /// </summary>
        public bool FadeInFadeOut { get; set; }

        /// <summary>
        /// Maximum duration of output video (in seconds).
        /// </summary>
        public double MaxMotionThumbnailDurationInSecs { get; set; }

        internal override string MethodName => "generatethumbnail";

        internal override IEnumerable<KeyValuePair<string, string>> GetQueryParameters()
        {
            yield return new KeyValuePair<string, string>("outputType", OutputType.ToString().ToLowerInvariant());
            yield return new KeyValuePair<string, string>("outputAudio", OutputAudio.ToString().ToLowerInvariant());
            yield return new KeyValuePair<string, string>("fadeInFadeOut", FadeInFadeOut.ToString().ToLowerInvariant());
            yield return new KeyValuePair<string, string>("maxMotionThumbnailDurationInSecs", MaxMotionThumbnailDurationInSecs.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// The types of output format.
        /// </summary>
        public enum OutputTypes
        {
            /// <summary>
            /// Video output
            /// </summary>
            Video
        }
    }
}
