/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Conditional probability result.
    /// </summary>
    [DataContract]
    public class ConditionalProbabilityResult
    {
        /// <summary>
        /// Gets or sets words.
        /// </summary>
        [DataMember(Name = "words")]
        public string Words { get; set; }

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