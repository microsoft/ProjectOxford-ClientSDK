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

using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.ProjectOxford.Common
{
    public abstract class ServiceClient
    {
        protected class UrlReqeust
        {
            public string url { get; set; }
        }

        public class WrappedClientError
        {
            public ClientError Error { get; set; }
        }
            
        #region private/protected members

        /// <summary>
        /// The header's key name of asset url.
        /// </summary>
        private static string OperationLocation = "Operation-Location";

        /// <summary>
        /// The default resolver.
        /// </summary>
        protected static CamelCasePropertyNamesContractResolver s_defaultResolver = new CamelCasePropertyNamesContractResolver();

        protected static JsonSerializerSettings s_settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = s_defaultResolver
        };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected ServiceClient() : this(new HttpClient())
        {
        }

        /// <summary>
        ///  Test constructor; use to inject mock clients.
        /// </summary>
        /// <param name="httpClient">Custom HttpClient, for testing.</param>
        protected ServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Root of the API URL.  Must not end in slash.
        /// </summary>
        protected string ApiRoot { get; set; }

        /// <summary>
        /// Header name for authorization.
        /// </summary>
        protected string AuthKey { get; set; }

        /// <summary>
        /// Header value for authorization.
        /// </summary>
        protected string AuthValue { get; set; }
        #endregion

        #region the JSON client
        /// <summary>
        /// Helper method executing a GET REST request.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <param name="apiUrl">API URL relative to the apiRoot</param>
        /// <param name="requestBody">Content of the HTTP request.</param>
        /// <returns>TResponse</returns>
        /// <exception cref="ClientException">Service exception</exception>
        protected async Task<TResponse> PostAsync<TRequest, TResponse>(string apiUrl, TRequest requestBody)
        {
            return await SendAsync<TRequest, TResponse>(HttpMethod.Post, apiUrl, requestBody);
        }

        /// <summary>
        /// Helper method executing a POST REST request.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <param name="apiUrl">API URL relative to the apiRoot</param>
        /// <param name="requestBody">Content of the HTTP request.</param>
        /// <returns>TResponse</returns>
        /// <exception cref="ClientException">Service exception</exception>
        protected async Task<TResponse> GetAsync<TRequest, TResponse>(string apiUrl, TRequest requestBody)
        {
            return await SendAsync<TRequest, TResponse>(HttpMethod.Get, apiUrl, requestBody);
        }

        /// <summary>
        /// Helper method executing a REST request.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <param name="method">HTTP method</param>
        /// <param name="apiUrl">API URL, generally relative to the ApiRoot</param>
        /// <param name="requestBody">Content of the HTTP request</param>
        /// <returns>TResponse</returns>
        /// <exception cref="ClientException">Service exception</exception>
        protected async Task<TResponse> SendAsync<TRequest, TResponse>(HttpMethod method, string apiUrl, TRequest requestBody)
        {
            bool urlIsRelative = System.Uri.IsWellFormedUriString(apiUrl, System.UriKind.Relative);

            string requestUri = urlIsRelative ? ApiRoot + apiUrl : apiUrl;
            var request = new HttpRequestMessage(method, requestUri);
            request.Headers.Add(AuthKey, AuthValue);

            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, s_settings), Encoding.UTF8, "application/json");
                }
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = null;
                if (response.Content != null)
                {
                    responseContent = await response.Content.ReadAsStringAsync();
                }

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return JsonConvert.DeserializeObject<TResponse>(responseContent, s_settings);
                }

                // For video submission, the response content is empty. The information is in the
                // response headers. 
                var output = System.Activator.CreateInstance<TResponse>();
                if (output is Contract.VideoOperation)
                {
                    var operation = output as Contract.VideoOperation;
                    operation.Url = response.Headers.GetValues(OperationLocation).First();
                    return output;
                }

                return default(TResponse);
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains("application/json"))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    var wrappedClientError = JsonConvert.DeserializeObject<WrappedClientError>(errorObjectString);
                    if (wrappedClientError?.Error != null)
                    {
                        throw new ClientException(wrappedClientError.Error, response.StatusCode);
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return default(TResponse);
        }
        #endregion
    }
}
