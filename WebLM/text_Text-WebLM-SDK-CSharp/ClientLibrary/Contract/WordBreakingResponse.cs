/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Word Breaking response.
    /// </summary>
    [DataContract]
    public class WordBreakingResponse
    {
        /// <summary>
        /// Gets or sets Word Breaking candidates.
        /// </summary>
        [DataMember(Name = "candidates")]
        public WordBreakingCandidate[] Candidates { get; set; }
    }
}