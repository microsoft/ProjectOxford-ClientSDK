/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Next word completion candidate.
    /// </summary>
    [DataContract]
    public class NextWordCompletionCandidate
    {
        /// <summary>
        /// Gets or sets word.
        /// </summary>
        [DataMember(Name = "word")]
        public string Word { get; set; }

        /// <summary>
        /// Gets or sets probability.
        /// </summary>
        [DataMember(Name = "probability")]
        public double Probability { get; set; }
    }
}