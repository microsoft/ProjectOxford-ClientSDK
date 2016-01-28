/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Conditional probability response.
    /// </summary>
    [DataContract]
    public class ConditionalProbabilityResponse
    {
        /// <summary>
        /// Gets or sets conditional probability results.
        /// </summary>
        [DataMember(Name = "results")]
        public ConditionalProbabilityResult[] Results { get; set; }
    }
}