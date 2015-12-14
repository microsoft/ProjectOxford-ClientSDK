// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Microsoft.ProjectOxford.Video.Contract
{
    /// <summary>
    /// Generic class for representing fragments.
    /// A fragment represents a span of time in the video.  Fragments will not overlap in time.
    /// </summary>
    /// <typeparam name="T">Type representing the event type specific to the action.</typeparam>
    public class Fragment<T>
    {
        /// <summary>
        /// Gets or sets timestamp of the first event, in ticks.
        /// </summary>
        public Int64 Start { get; set; }

        /// <summary>
        /// Gets or sets overall duration of the fragment, in ticks.
        /// </summary>
        public Int64 Duration { get; set; }

        /// <summary>
        /// Gets or sets the duration of each event, in ticks.  Optional if no events are present.
        /// Note: Duration = Interval * Events.length.
        /// </summary>
        public Int64? Interval { get; set; }

        /// <summary>
        /// Gets or sets array of list of events.  Each array entry is Interval units long in ticks.
        /// </summary>
        public T[][] Events { get; set; }
    }
}
