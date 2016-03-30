// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Settings for motion detection operation.
    /// </summary>
    public class MotionDetectionOperationSettings : VideoOperationSettings
    {
        internal override string MethodName => "detectmotion";

        internal override IEnumerable<KeyValuePair<string, string>> GetQueryParameters()
        {
            yield break;
        }
    }
}
