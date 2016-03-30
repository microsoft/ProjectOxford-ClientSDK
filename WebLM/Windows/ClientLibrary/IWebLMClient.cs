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

using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Text.WebLM
{
    /// <summary>
    /// WebLM client interface.
    /// </summary>
    public interface IWebLMClient
    {
        /// <summary>
        /// Calculate Joint Probabilities asynchronously.
        /// </summary>
        /// <param name="queries">Joint probability queries</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <returns>Joint probabilities response.</returns>
        Task<JointProbabilityResponse> CalculateJointProbabilitiesAsync(string[] queries, string model, int order = DefaultValues.OrderDefault);

        /// <summary>
        /// Calculate Conditional Probabilities asynchronously.
        /// </summary>
        /// <param name="queries">Conditional probability queries</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <returns>Conditional probabilities response.</returns>
        Task<ConditionalProbabilityResponse> CalculateConditionalProbabilitiesAsync(ConditionalProbabilityQuery[] queries, string model, int order = DefaultValues.OrderDefault);

        /// <summary>
        /// Generate next words asynchronously.
        /// </summary>
        /// <param name="words">A string containing a sequence of words from which to generate the list of words likely to follow.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <param name="maxNumOfCandidatesReturned">Max number of candidates would be returned.</param>
        /// <returns>Next word completions response.</returns>
        Task<NextWordCompletionResponse> GenerateNextWordsAsync(string words, string model, int order = DefaultValues.OrderDefault, int maxNumOfCandidatesReturned = DefaultValues.CandidatesDefault);

        /// <summary>
        /// Break into words asynchronously.
        /// </summary>
        /// <param name="text">The line of text to break into words.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <param name="maxNumOfCandidatesReturned">Max number of candidates would be returned.</param>
        /// <returns>Word breaking response.</returns>
        Task<WordBreakingResponse> BreakIntoWordsAsync(string text, string model, int order = DefaultValues.OrderDefault, int maxNumOfCandidatesReturned = DefaultValues.CandidatesDefault);

        /// <summary>
        /// List available models asynchronously.
        /// </summary>
        /// <returns>List available models response.</returns>
        Task<ListAvailableModelsResponse> ListAvailableModelsAsync();
    }
}
