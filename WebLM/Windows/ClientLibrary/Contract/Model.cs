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

using System.Runtime.Serialization;

namespace Microsoft.ProjectOxford.Text.WebLM
{
    [DataContract]
    public class Model
    {
        /// <summary>
        /// Gets or sets the details of the corpus that the model was trained on.
        /// </summary>
        [DataMember(Name = "corpus")]
        public string Corpus { get; set; }

        /// <summary>
        /// Gets or sets the model's name.
        /// </summary>
        [DataMember(Name = "model")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the model's maximum order.
        /// </summary>
        [DataMember(Name = "maxOrder")]
        public int MaxOrder { get; set; }

        /// <summary>
        /// Gets or sets the model's supported operations.
        /// </summary>
        [DataMember(Name = "supportedOperations")]
        public SupportedOperations[] SupportedOperations { get; set; }
    }
}
