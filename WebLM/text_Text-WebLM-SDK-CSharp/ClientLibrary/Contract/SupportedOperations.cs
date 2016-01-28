/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [DataContract]
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SupportedOperations
    {
        [EnumMember]
        CalculateJointProbability,

        [EnumMember]
        CalculateConditionalProbability,

        [EnumMember]
        GenerateNextWords,

        [EnumMember]
        BreakIntoWords
    }
}
