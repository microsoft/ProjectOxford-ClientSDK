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

using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.EntityLinking.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.ProjectOxford.EntityLinking
{
    /// <summary>
    /// The EntityLinking service client proxy implementation.
    /// </summary>
    public class EntityLinkingServiceClient : IEntityLinkingServiceClient
    {
        #region private members

        /// <summary>
        /// The json header
        /// </summary>
        private const string Header = "text/plain";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "subscription-key";

        /// <summary>
        /// Path string for REST EntityLink recognition method.
        /// </summary>
        private const string LinkQuery = "link";

        /// <summary>
        /// The subscription key.
        /// </summary>
        private readonly string _subscriptionKey;

        /// <summary>
        /// The default resolver.
        /// </summary>
        private static CamelCasePropertyNamesContractResolver s_defaultResolver = new CamelCasePropertyNamesContractResolver();

        private static JsonSerializerSettings s_settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = s_defaultResolver
        };

        private static HttpClient s_httpClient = new HttpClient();
        private readonly HttpClient _httpClient;

        private static string s_apiRoot = "https://api.projectoxford.ai";
        private readonly string _serviceUrl;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLinkingServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public EntityLinkingServiceClient(string subscriptionKey) : this(s_httpClient, subscriptionKey, s_apiRoot) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLinkingServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Host name of the service URL, without the trailing slash.</param>
        public EntityLinkingServiceClient(string subscriptionKey, string apiRoot) : this(s_httpClient, subscriptionKey, apiRoot) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLinkingServiceClient"/> class, with a client-supplied
        /// HttpClient object. Intended primarily for testing.
        /// </summary>
        /// <param name="httpClient">the HttpClient object</param>
        /// <param name="subscriptionKey">The subscription key.</param>
        public EntityLinkingServiceClient(HttpClient httpClient, string subscriptionKey) : this(httpClient, subscriptionKey, s_apiRoot) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityLinkingServiceClient"/> class, with a client-supplied
        /// HttpClient object. Intended primarily for testing.
        /// </summary>
        /// <param name="httpClient">the HttpClient object</param>
        /// <param name="subscriptionKey">The subscription key.</param>
        public EntityLinkingServiceClient(HttpClient httpClient, string subscriptionKey, string apiRoot)
        {
            _httpClient = httpClient;
            _subscriptionKey = subscriptionKey;
            _serviceUrl = apiRoot + "/entitylinking/v1.0";
        }

        #region IEntityLinkingServiceClient implementations

        /// <summary>
        /// Linking entities in a text.
        /// </summary>
        /// <param name="text">the text.</param>
        /// <param name="selection">the specific word or phrase within the text that is to be entity linked. If not specified, the service will try to recognize and identify all the entities within the input text.</param>
        /// <param name="offset">the location (in offset by characters) of the selected word or phrase within the input text. Used to distinguish when there are multiple instances of the same words or phrases within the input text. Only valid when the selection is specified.</param>
        /// <returns>Async task, which, upon completion, will return EntityLinks.</returns>
        public async Task<Contract.EntityLink[]> LinkAsync(string text, string selection="", int offset=0)
        {
            return await SendRequestAsync(text, selection, offset);
        }

        #endregion

        #region the JSON client
        /// <summary>
        /// Helper method executing the REST request.
        /// </summary>
        /// <param name="requestBody">Content of the HTTP request.</param>
        /// <returns></returns>
        private async Task<Contract.EntityLink[]> SendRequestAsync(string requestBody, string selection, int offset)
        {
            var httpMethod = HttpMethod.Post;
            var requestUri = new StringBuilder();
            requestUri.AppendFormat("{0}/{1}", _serviceUrl, LinkQuery);
            requestUri.Append('?');

            requestUri.AppendFormat("{0}={1}", SubscriptionKeyName, _subscriptionKey);

            if (!string.IsNullOrEmpty(selection))
            {
                requestUri.Append($"&selection={Uri.EscapeDataString(selection)}");
                if (offset > 0)
                {
                    requestUri.Append($"&offset={offset}");
                }
            }

            var request = new HttpRequestMessage(httpMethod, _serviceUrl);
            request.RequestUri = new Uri(requestUri.ToString());

            if (requestBody != null)
            {
                request.Content = new StringContent(requestBody, Encoding.UTF8, Header);
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
                    var ret = JsonConvert.DeserializeObject<EntityLinkResponse>(responseContent, s_settings);
                    return ret?.EntityLinks;
                }

                return null;
            }
            else
            {
                if (response.Content != null)
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(errorObjectString);
                    if (errorResponse?.Error != null)
                    {
                        throw new ClientException(errorResponse.Error, response.StatusCode);
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return null;
        }


        #endregion
    }
}
