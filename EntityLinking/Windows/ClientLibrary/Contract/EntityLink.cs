// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
// 
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/ProjectOxford-ClientSDK
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.ProjectOxford.Common;

namespace Microsoft.ProjectOxford.EntityLinking.Contract
{
    /// <summary>
    /// Container of response.
    /// </summary>
    [DataContract]
    internal class EntityLinkResponse
    {
        /// <summary>
        /// Gets or sets EntityLinks.
        /// </summary>
        [DataMember(EmitDefaultValue = false, Name = "entities")]
        public EntityLink[] EntityLinks { get; set; }
    }

    /// <summary>
    /// Represents name of the entity, wikipedia id, matches and score.
    /// </summary>
    [DataContract]
    public class EntityLink
    {
        /// <summary>
        /// Name of the entity.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Id of the wikipedia linked entity.
        /// </summary>
        [DataMember(Name = "wikipediaId")]
        public string WikipediaID { get; set; }

        /// <summary>
        /// Matches text of the entity within the given paragraph.
        /// </summary>
        [DataMember(Name = "matches")]
        public IList<Match> Matches { get; set; }

        /// <summary>
        /// Confidence score of the linking.
        /// </summary>
        [DataMember(Name = "score")]
        public double Score { get; set; }
    }

    /// <summary>
    /// The match text of the knowledge base linked entity.
    /// </summary>
    [DataContract]
    public class Match
    {
        /// <summary>
        /// The matched text.
        /// </summary>
        [DataMember(Name = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Matched entries within the given paragraph.
        /// </summary>
        [DataMember(Name = "entries")]
        public IList<Entry> Entries { get; set; }
    }

    /// <summary>
    /// Represents the entry offset and score.
    /// </summary>
    [DataContract]
    public class Entry
    {
        /// <summary>
        /// Offset of the entry.
        /// </summary>
        [DataMember(Name = "offset")]
        public int Offset { get; set; }

        /// <summary>
        /// The score of the entry.
        /// </summary>
        [DataMember(Name = "score")]
        public double Score { get; set; }
    }


}
