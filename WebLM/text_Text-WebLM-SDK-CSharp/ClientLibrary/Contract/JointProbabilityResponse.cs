/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Joint probability response.
    /// </summary>
    [DataContract]
    public class JointProbabilityResponse
    {
        /// <summary>
        /// Gets or sets joint probability results.
        /// </summary>
        [DataMember(Name = "results")]
        public JointProbabilityResult[] Results { get; set; }
    }
}