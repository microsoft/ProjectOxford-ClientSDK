/********************************************************
*                                                        *
*   Copyright (c) Microsoft. All rights reserved.        *
*                                                        *
*********************************************************/

namespace Microsoft.ProjectOxford.Text.WebLM
{
    using System.Threading.Tasks;

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
