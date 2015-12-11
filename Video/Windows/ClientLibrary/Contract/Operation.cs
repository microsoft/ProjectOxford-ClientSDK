// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// The video operation.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Operation"/> class.
        /// </summary>
        /// <param name="url">The operation url.</param>
        public Operation(string url)
        {
            Url = url;
        }

        /// <summary>
        /// Gets operation url.
        /// </summary>
        public string Url { get; private set; }
    }
}
