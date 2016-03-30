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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Linguistics.Contract;

namespace Microsoft.ProjectOxford.Linguistics
{
    public class LinguisticsClient : ILinguisticsClient
    {
        #region private members

        /// <summary>
        /// The Default Service Host
        /// </summary>
        private const string DefaultServiceHost = "https://api.projectoxford.ai/linguistics/v1.0";

        /// <summary>
        /// The JSON content type header.
        /// </summary>
        private const string JsonContentTypeHeader = "application/json";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "ocp-apim-subscription-key";

        /// <summary>
        /// The ListAnalyzers.
        /// </summary>
        private const string ListAnalyzersQuery = "analyzers";

        /// <summary>
        /// The AnalyzeText.
        /// </summary>
        private const string AnalyzeTextQuery = "analyze";

        /// <summary>
        /// The default resolver.
        /// </summary>
        private static readonly CamelCasePropertyNamesContractResolver defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// The settings
        /// </summary>
        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = defaultResolver
        };

        /// <summary>
        /// The service host.
        /// </summary>
        private string serviceHost;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private HttpClient httpClient;
        #endregion

        public LinguisticsClient(string subscriptionKey, string serviceHost = null)
        {
            this.serviceHost = string.IsNullOrWhiteSpace(serviceHost) ? DefaultServiceHost : serviceHost.Trim();

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add(SubscriptionKeyName, subscriptionKey);
        }

        #region ILinguisticsClient implementations
        /// <summary>
        /// List analyzers asynchronously.
        /// </summary>
        /// <returns>An array of supported analyzers.</returns>
        public async Task<Analyzer[]> ListAnalyzersAsync()
        {
            var requestUrl = $"{this.serviceHost}/{ListAnalyzersQuery}";

            return await this.SendRequestAsync<object, Analyzer[]>(HttpMethod.Get, requestUrl);
        }

        /// <summary>
        /// Analyze text asynchronously.
        /// </summary>
        /// <param name="request">Analyze text request.</param>
        /// <returns>An array of analyze text result.</returns>
        public async Task<AnalyzeTextResult[]> AnalyzeTextAsync(AnalyzeTextRequest request)
        {
            var requestUrl = $"{this.serviceHost}/{AnalyzeTextQuery}";

            return await this.SendRequestAsync<object, AnalyzeTextResult[]>(HttpMethod.Post, requestUrl, request);
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
            var request = new HttpRequestMessage(httpMethod, requestUrl);
            if (requestBody != null)
            {
                request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, settings), Encoding.UTF8, JsonContentTypeHeader);
            }

            HttpResponseMessage response = await httpClient.SendAsync(request);
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