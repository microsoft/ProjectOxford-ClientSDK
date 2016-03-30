using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Common.Contract
{
    public abstract class VideoOperationResult
    {
        public VideoOperationResult()
        {

        }

        /// <summary>
        /// Protected copy constructor
        /// </summary>
        /// <param name="other">Result from which to copy.</param>
        protected VideoOperationResult(VideoOperationResult other)
        {
            Status = other.Status;
            CreatedDateTime = other.CreatedDateTime;
            LastActionDateTime = other.LastActionDateTime;
            Message = other.Message;
            ResourceLocation = other.ResourceLocation;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public VideoOperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the created date time.
        /// </summary>
        /// <value>
        /// The created date time.
        /// </value>
        public DateTime CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the last action date time.
        /// </summary>
        /// <value>
        /// The last action date time.
        /// </value>
        public DateTime LastActionDateTime { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the resource location.
        /// </summary>
        /// <value>
        /// The resource location.
        /// </value>
        public string ResourceLocation { get; set; }
    }
}
