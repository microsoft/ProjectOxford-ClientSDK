/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Conditional probability query.
    /// </summary>
    [DataContract]
    public class ConditionalProbabilityQuery
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
    }
}