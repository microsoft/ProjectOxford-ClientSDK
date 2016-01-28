/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Next word completion response.
    /// </summary>
    [DataContract]
    public class NextWordCompletionResponse
    {
        /// <summary>
        /// Gets or sets next word completion candidates.
        /// </summary>
        [DataMember(Name = "candidates")]
        public NextWordCompletionCandidate[] Candidates { get; set; }
    }
}