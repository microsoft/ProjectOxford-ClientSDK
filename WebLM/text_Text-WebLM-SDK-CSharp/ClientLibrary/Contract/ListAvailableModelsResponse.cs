/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    /// <summary>
    /// List available models response.
    /// </summary>
    [DataContract]
    public class ListAvailableModelsResponse
    {
        /// <summary>
        /// Gets or sets models.
        /// </summary>
        [DataMember(Name = "models")]
        public Model[] Models { get; set; }
    }
}
