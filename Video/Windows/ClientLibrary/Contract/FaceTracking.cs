// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Microsoft.ProjectOxford.Video.Contract
{
    public class FaceTracking : ResultBase
    {
        /// <summary>
        /// Gets or sets the fragments.
        /// </summary>
        public Fragment<FaceEvent>[] Fragments { get; set; }

        /// <summary>
        /// Gets or sets the faces detected.
        /// </summary>
        public Face[] FacesDetected { get; set; }
    }
}
