// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK Github:
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

using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Video.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Video
{
    /// <summary>
    /// The video service client.
    /// </summary>
    public class VideoServiceClient : IVideoServiceClient
    {
        #region private members
        /// <summary>
        /// The service host.
        /// </summary>
        private const string ServiceHost = "https://api.projectoxford.ai/video/v1.0";

        /// <summary>
        /// The JSON content type header.
        /// </summary>
        private const string JsonContentTypeHeader = "application/json";

        /// <summary>
        /// The stream content type header.
        /// </summary>
        private const string StreamContentTypeHeader = "application/octet-stream";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "ocp-apim-subscription-key";

        /// <summary>
        /// The header's key name of asset url.
        /// </summary>
        private const string OperationLocation = "Operation-Location";

        /// <summary>
        /// The processing result.
        /// </summary>
        private const string ProcessingResult = "processingResult";

        /// <summary>
        /// The settings
        /// </summary>
        private static JsonSerializerSettings s_settings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// The HTTP client
        /// </summary>
        private HttpClient _httpClient = new HttpClient();
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public VideoServiceClient(string subscriptionKey)
        {
            _httpClient.DefaultRequestHeaders.Add(SubscriptionKeyName, subscriptionKey);
        }

        #region APIS
        /// <summary>
        /// Create video operation.
        /// </summary>
        /// <param name="video">Video stream.</param>
        /// <param name="operationType">Operation type.</param>
        /// <returns>Video operation created.</returns>
        public async Task<Operation> CreateOperationAsync(Stream video, OperationType operationType)
        {
            var url = string.Format("{0}/{1}", ServiceHost, operationType.ToString().ToLowerInvariant());
            var response = await SendRequestAsync(HttpMethod.Post, url, video);
            Operation operation = new Operation(response.Headers.GetValues(OperationLocation).First());
            return operation;
        }

        /// <summary>
        /// Create video operation.
        /// </summary>
        /// <param name="video">Video byte array.</param>
        /// <param name="operationType">Operation status url.</param>
        /// <returns>Video operation created.</returns>
        public async Task<Operation> CreateOperationAsync(byte[] video, OperationType operationType)
        {
            var url = string.Format("{0}/{1}", ServiceHost, operationType.ToString().ToLowerInvariant());
            var response = await SendRequestAsync(HttpMethod.Post, url, video);
            Operation operation = new Operation(response.Headers.GetValues(OperationLocation).First());
            return operation;
        }

        /// <summary>
        /// Create video operation.
        /// </summary>
        /// <param name="videoUrl">Video url.</param>
        /// <param name="operationType">>Operation type.</param>
        /// <returns>Video operation created.</returns>
        public async Task<Operation> CreateOperationAsync(string videoUrl, OperationType operationType)
        {
            var url = string.Format("{0}/{1}", ServiceHost, operationType.ToString().ToLowerInvariant());
            var response = await SendRequestAsync(HttpMethod.Post, url, new VideoUrlRequest() { Url = videoUrl });
            Operation operation = new Operation(response.Headers.GetValues(OperationLocation).First());
            return operation;
        }

        /// <summary>
        /// Get video operation result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>Operation status.</returns>
        public async Task<OperationResult> GetOperationResultAsync(Operation operation)
        {
            var response = await SendRequestAsync(HttpMethod.Get, operation.Url, null);
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<OperationResult>(responseContent as string, s_settings);
        }

        /// <summary>
        /// Get result video.
        /// </summary>
        /// <param name="url">The result video content url.</param>
        /// <returns>The result video stream.</returns>
        public async Task<Stream> GetResultVideoAsync(string url)
        {
            var response = await SendRequestAsync(HttpMethod.Get, url, null);
            return await response.Content.ReadAsStreamAsync();
        }
        #endregion

        #region the json client
        /// <summary>
        /// Sends the request asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The response.</returns>
        /// <exception cref="ClientException">The client exception.</exception>
        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod httpMethod, string requestUrl, object requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, requestUrl);
            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(StreamContentTypeHeader);
                }
                else if (requestBody is byte[])
                {
                    request.Content = new ByteArrayContent(requestBody as byte[]);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(StreamContentTypeHeader);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, s_settings), Encoding.UTF8, JsonContentTypeHeader);
                }
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonContentTypeHeader))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    var errorCollection = JsonConvert.DeserializeObject<ServiceError>(errorObjectString);
                    if (errorCollection != null)
                    {
                        throw new ClientException(errorCollection.Error, response.StatusCode);
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return response;
        }
        #endregion
    }
}
