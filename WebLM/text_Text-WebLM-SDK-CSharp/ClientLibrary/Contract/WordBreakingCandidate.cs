/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Word Breaking Candidate.
    /// </summary>
    [DataContract]
    public class WordBreakingCandidate
    {
        /// <summary>
        /// Gets or sets words.
        /// </summary>
        [DataMember(Name = "words")]
        public string Words { get; set; }

        /// <summary>
        /// Gets or sets probability.
        /// </summary>
        [DataMember(Name = "probability")]
        public double Probability { get; set; }
    }
}