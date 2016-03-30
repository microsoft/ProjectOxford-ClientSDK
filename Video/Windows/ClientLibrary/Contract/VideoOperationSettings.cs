// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Base class of video operation settings.
    /// </summary>
    public abstract class VideoOperationSettings
    {
        internal abstract string MethodName { get; }

        internal abstract IEnumerable<KeyValuePair<string, string>> GetQueryParameters();
    }
}
