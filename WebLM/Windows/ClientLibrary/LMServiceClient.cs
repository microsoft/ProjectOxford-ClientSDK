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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.ProjectOxford.Text.WebLM
{
    public class LMServiceClient : IWebLMClient
    {
        #region private members

        /// <summary>
        /// The Service Host
        /// </summary>
        private const string ServiceHost = "https://api.projectoxford.ai/text/weblm/v1.0";

        /// <summary>
        /// The JSON content type header.
        /// </summary>
        private const string JsonContentTypeHeader = "application/json";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "ocp-apim-subscription-key";

        /// <summary>
        /// The JointProbability.
        /// </summary>
        private const string CalculateJointProbabilityQuery = "calculateJointProbability";

        /// <summary>
        /// The ConditionalProbability.
        /// </summary>
        private const string CalculateConditionalProbabilityQuery = "calculateConditionalProbability";

        /// <summary>
        /// The NextWordCompletions.
        /// </summary>
        private const string GenerateNextWordsQuery = "generateNextWords";

        /// <summary>
        /// The WordBreakings.
        /// </summary>
        private const string BreakIntoWordsQuery = "breakIntoWords";

        /// <summary>
        /// The ListAvailableModels.
        /// </summary>
        private const string ListAvailableModelsQuery = "models";

        /// <summary>
        /// The default resolver.
        /// </summary>
        private static CamelCasePropertyNamesContractResolver defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// The settings
        /// </summary>
        private static JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = defaultResolver
        };

        /// <summary>
        /// The subscription key.
        /// </summary>
        private string _subscriptionKey;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private HttpClient _httpLMServiceClient;
        #endregion

        public LMServiceClient(string subscriptionKey)
        {
            this._subscriptionKey = subscriptionKey;
            _httpLMServiceClient = new HttpClient();
            _httpLMServiceClient.DefaultRequestHeaders.Add(SubscriptionKeyName, subscriptionKey);
        }

        #region INgramLookupClient implementations
        /// <summary>
        /// Calculate Joint Probabilities asynchronously.
        /// </summary>
        /// <param name="queries">Joint probability queries.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <returns>Joint probabilities response.</returns>
        public async Task<JointProbabilityResponse> CalculateJointProbabilitiesAsync(string[] queries, string model, int order = DefaultValues.OrderDefault)
        {
            if ((queries == null) || (queries.Length == 0) || (model == null) || (model.Length == 0) || (order <= 0))
                throw new ArgumentException("Arguments must be valid.");

            var requestUrl = string.Format(
                "{0}/{1}?model={2}&order={3}",
                ServiceHost,
                CalculateJointProbabilityQuery,
                model,
                order);

            var jointProbabilityRequest = new { queries = queries };
            return await this.SendRequestAsync<object, JointProbabilityResponse>(HttpMethod.Post, requestUrl, jointProbabilityRequest);
        }

        /// <summary>
        /// Calculate Conditional Probabilities asynchronously.
        /// </summary>
        /// <param name="queries">Conditional probability queries.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <returns>Conditional probabilities response.</returns>
        public async Task<ConditionalProbabilityResponse> CalculateConditionalProbabilitiesAsync(ConditionalProbabilityQuery[] queries, string model, int order = DefaultValues.OrderDefault)
        {
            if ((queries == null) || (queries.Length == 0) || (model == null) || (model.Length == 0) || (order <= 0))
                throw new ArgumentException("Arguments must be valid.");

            var requestUrl = string.Format(
                "{0}/{1}?model={2}&order={3}",
                ServiceHost,
                CalculateConditionalProbabilityQuery,
                model,
                order);

            var conditionalProbabilityRequest = new { queries = queries };
            return await this.SendRequestAsync<object, ConditionalProbabilityResponse>(HttpMethod.Post, requestUrl, conditionalProbabilityRequest);
        }

        /// <summary>
        /// Generate next words asynchronously.
        /// </summary>
        /// <param name="words">A string containing a sequence of words from which to generate the list of words likely to follow.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <param name="maxNumOfCandidatesReturned">Max number of candidates would be returned.</param>
        /// <returns>Next word completions response.</returns>
        public async Task<NextWordCompletionResponse> GenerateNextWordsAsync(string words, string model, int order = DefaultValues.OrderDefault, int maxNumOfCandidatesReturned = DefaultValues.CandidatesDefault)
        {
            if ((words == null) || (words.Length == 0) || (model == null) || (model.Length == 0) || (order <= 0))
                throw new ArgumentException("Arguments must be valid.");

            var requestUrl = string.Format(
                "{0}/{1}?model={2}&words={3}&order={4}&maxNumOfCandidatesReturned={5}",
                ServiceHost,
                GenerateNextWordsQuery,
                model,
                words,
                order,
                maxNumOfCandidatesReturned);

            return await this.SendRequestAsync<object, NextWordCompletionResponse>(HttpMethod.Post, requestUrl);
        }

        /// <summary>
        /// Break into words asynchronously.
        /// </summary>
        /// <param name="text">The line of text to break into words.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <param name="maxNumOfCandidatesReturned">Max number of candidates would be returned.</param>
        /// <returns>Word breaking response.</returns>
        public async Task<WordBreakingResponse> BreakIntoWordsAsync(string text, string model, int order = DefaultValues.OrderDefault, int maxNumOfCandidatesReturned = DefaultValues.CandidatesDefault)
        {
            if ((text == null) || (text.Length == 0) || (model == null) || (model.Length == 0) || (order <= 0))
                throw new ArgumentException("Arguments must be valid.");

            var requestUrl = string.Format(
                "{0}/{1}?model={2}&text={3}&order={4}&maxNumOfCandidatesReturned={5}",
                ServiceHost,
                BreakIntoWordsQuery,
                model,
                text,
                order,
                maxNumOfCandidatesReturned);

            return await this.SendRequestAsync<object, WordBreakingResponse>(HttpMethod.Post, requestUrl);
        }

        /// <summary>
        /// List available models asynchronously.
        /// </summary>
        /// <returns>List available models response.</returns>
        public async Task<ListAvailableModelsResponse> ListAvailableModelsAsync()
        {
            var requestUrl = string.Format(
                "{0}/{1}",
                ServiceHost,
                ListAvailableModelsQuery
                );

            return await this.SendRequestAsync<object, ListAvailableModelsResponse>(HttpMethod.Get, requestUrl);
        }
        #endregion

        #region the json client
        /// <summary>
        /// Sends the request asynchronously.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The response.</returns>
        /// <exception cref="ClientException">The client exception.</exception>
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody = default(TRequest))
        {
            var request = new HttpRequestMessage(httpMethod, ServiceHost);
            request.RequestUri = new Uri(requestUrl);
            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, settings), Encoding.UTF8, JsonContentTypeHeader);
            }

            HttpResponseMessage response = await _httpLMServiceClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, settings);
                }

                return default(TResponse);
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonContentTypeHeader))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ClientError errorCollection = JsonConvert.DeserializeObject<ClientError>(errorObjectString);
                    if (errorCollection != null)
                    {
                        throw new ClientException(errorCollection, response.StatusCode);
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return default(TResponse);
        }
        #endregion
    }
}
