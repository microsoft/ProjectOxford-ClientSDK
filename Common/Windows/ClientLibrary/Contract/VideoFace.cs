using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Common.Contract
{
    /// <summary>
    /// Face object returned as part of the FaceDetection/EmotionRecognition operations.
    /// </summary>
    public class VideoFace
    {
        /// <summary>
        /// Gets or sets Id of face.
        /// </summary>
        public int FaceId { get; set; }
    }
}
