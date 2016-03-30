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

namespace Microsoft.ProjectOxford.Linguistics.Contract
{
    /// <summary>
    /// Represents a single analysis operation that can be performed on text, like finding sentence boundaries or part-of-speech tags.
    /// </summary>
    public class Analyzer
    {
        /// <summary>
        /// Unique identifier for this analyzer used to communicate with the service
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// List of two letter ISO language codes for which this analyzer is available. e.g. "en" represents "English"
        /// </summary>
        public string[] Languages { get; set; }

        /// <summary>
        /// Description of the type of analysis used here, such as Constituency_Tree or POS_tags.
        /// </summary>
        public string Kind { get; set; }

        /// <summary>
        /// The specification for how a human should produce ideal output for this task. Most use the specification from the Penn Teeebank.
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// Description of the implementaiton used in this analyzer.
        /// </summary>
        public string Implementation { get; set; }
    }
}