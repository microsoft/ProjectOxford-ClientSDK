// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Project Oxford: http://ProjectOxford.ai
// 
// Project Oxford SDK Github:
// https://github.com/Microsoft/ProjectOxfordSDK-Windows
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
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.ProjectOxford.Emotion
{
    public class EmotionServiceClient : IEmotionServiceClient
    {
        #region private members

        /// <summary>
        /// The json header
        /// </summary>
        private const string JsonHeader = "application/json";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "subscription-key";

        /// <summary>
        /// Path string for REST Emotion recognition method.
        /// </summary>
        private const string RecognizeQuery = "recognize";

        /// <summary>
        /// Optional query string for REST Emotion recognition method.
        /// </summary>
        private const string FaceRectangles = "faceRectangles";

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
        /// Initializes a new instance of the <see cref="EmotionServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public EmotionServiceClient(string subscriptionKey) : this(s_httpClient, subscriptionKey, s_apiRoot) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmotionServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="apiRoot">Host name of the service URL, without the trailing slash.</param>
        public EmotionServiceClient(string subscriptionKey, string apiRoot) : this(s_httpClient, subscriptionKey, apiRoot) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmotionServiceClient"/> class, with a client-supplied
        /// HttpClient object. Intended primarily for testing.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="subscriptionKey"></param>
        public EmotionServiceClient(HttpClient httpClient, string subscriptionKey) : this(httpClient, subscriptionKey, s_apiRoot) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmotionServiceClient"/> class, with a client-supplied
        /// HttpClient object. Intended primarily for testing.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="subscriptionKey"></param>
        /// <param name="subscriptionKey"></param>
        public EmotionServiceClient(HttpClient httpClient, string subscriptionKey, string apiRoot)
        {
            _httpClient = httpClient;
            _subscriptionKey = subscriptionKey;
            _serviceUrl = apiRoot + "/emotion/v1.0";
        }

        #region IEmotionServiceClient implementations
        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        public async Task<Contract.Emotion[]> RecognizeAsync(String imageUrl)
        {
            return await RecognizeAsync(imageUrl, null);
        }

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <param name="faceRectangles">Array of face rectangles.</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        public async Task<Contract.Emotion[]> RecognizeAsync(String imageUrl, Rectangle[] faceRectangles)
        {
            return await SendRequestAsync<object, Contract.Emotion[]>(faceRectangles, new { url = imageUrl });
        }

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageStream">Stream of the image</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        public async Task<Contract.Emotion[]> RecognizeAsync(Stream imageStream)
        {
            return await RecognizeAsync(imageStream, null);
        }

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageStream">Stream of the image</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each face.</returns>        
        public async Task<Contract.Emotion[]> RecognizeAsync(Stream imageStream, Rectangle[] faceRectangles)
        {
            return await SendRequestAsync<Stream, Contract.Emotion[]>(faceRectangles, imageStream);
        }
        #endregion

        #region the JSON client
        /// <summary>
        /// Helper method executing the REST request.
        /// </summary>
        /// <typeparam name="TRequest">Type of request.</typeparam>
        /// <typeparam name="TResponse">Type of response.</typeparam>
        /// <param name="faceRectangles">Optional list of face rectangles.</param>
        /// <param name="requestBody">Content of the HTTP request.</param>
        /// <returns></returns>
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(Rectangle[] faceRectangles, TRequest requestBody)
        {
            var httpMethod = HttpMethod.Post;
            var requestUri = new StringBuilder();
            requestUri.AppendFormat("{0}/{1}", _serviceUrl, RecognizeQuery);
            requestUri.Append('?');
            if (faceRectangles != null)
            {
                requestUri.Append(FaceRectangles);
                requestUri.Append('=');
                foreach (var rectangle in faceRectangles)
                {
                    requestUri.AppendFormat("{0},{1},{2},{3};",
                        rectangle.Left,
                        rectangle.Top,
                        rectangle.Width,
                        rectangle.Height);
                }
                requestUri.Remove(requestUri.Length - 1, 1); // drop last comma
                requestUri.Append('&');
            }
            requestUri.AppendFormat("{0}={1}",
                SubscriptionKeyName,
                _subscriptionKey);

            var request = new HttpRequestMessage(httpMethod, _serviceUrl);
            request.RequestUri = new Uri(requestUri.ToString());

            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, s_settings), Encoding.UTF8, JsonHeader);
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

                return default(TResponse);
            }
            else
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonHeader))
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
