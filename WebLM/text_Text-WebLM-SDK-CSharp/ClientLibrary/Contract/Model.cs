/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Model
    {
        [DataMember(Name = "corpus")]
        public string Corpus { get; set; }

        [DataMember(Name = "model")]
        public string Name { get; set; }

        [DataMember(Name = "maxOrder")]
        public int MaxOrder { get; set; }

        [DataMember(Name = "supportedOperations")]
        public SupportedOperations[] SupportedOperations { get; set; }
    }
}
