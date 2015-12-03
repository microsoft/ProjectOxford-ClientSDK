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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO;
using System.Web;
using System.Net;

namespace SPIDIdentificationAPI_WPF_Samples
{
    /// <summary>
    /// This class abstracts all the identification service calls
    /// </summary>
    public class IdentificationServiceHttpClientHelper
    {
        private string _subscriptionKey;
        private const string IDENTIFICATION_PROFILE_URI = "https://api.projectoxford.ai/spid/v1.0/identificationProfiles";
        private const string IDENTIFICATION_URI = "https://api.projectoxford.ai/spid/v1.0/identify";
        private const string SUBSCRIPTION_KEY_HEADER = "Ocp-Apim-Subscription-Key";
        private const string JSON_CONTENT_HEADER_VALUE = "application/json";
        private const string STREAM_CONTENT_HEADER_VALUE = "application/octet-stream";
        private const string OPERATION_STATUS_SUCCEEDED = "succeeded";
        private const string OPERATION_LOCATION_HEADER = "Operation-Location";
        private const int OPERATION_STATUS_UPDATE_DELAY = 5000;

        /// <summary>
        /// Initializes an instance of the helper
        /// </summary>
        /// <param name="subscriptionKey">The subscription key to use</param>
        public IdentificationServiceHttpClientHelper(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// Create a new profile asynchronously
        /// </summary>
        /// <param name="userData">The user data associated with the profile</param>
        /// <param name="locale">The profile locale</param>
        /// <returns>The response of the profile create request</returns>
        public async Task<CreateProfileResponse> CreateProfileAsync(string locale)
        {
            using (HttpClient client = new HttpClient())
            {
                // Construct the request header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT_HEADER_VALUE));
                client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                // Construct the request body
                var content = new FormUrlEncodedContent(new[]
                 {
                        new KeyValuePair<string, string>("locale", locale),
                 });

                // Send the request
                var response = await client.PostAsync(IDENTIFICATION_PROFILE_URI, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    string resultStr = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CreateProfileResponse>(resultStr);
                }
                else
                {
                    throw new Exception("Cannot create profile: " + response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Enrolls a profile with an audio stream
        /// </summary>
        /// <param name="audioStream">The audio stream to use for enrollment</param>
        /// <param name="profileId">The profile ID to enroll</param>
        /// <returns>The enrollment response</returns>
        public async Task<EnrollmentResponse> EnrollAsync(Stream audioStream, string profileId)
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                // Construct the request header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(STREAM_CONTENT_HEADER_VALUE));
                client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                // Construct the request body
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString());
                content.Add(new StreamContent(audioStream), "enrollmentData", profileId + "_" + DateTime.Now.ToString());

                // Send the request
                profileId = Uri.EscapeDataString(profileId);
                string requestUri = IDENTIFICATION_PROFILE_URI + "/" + profileId + "/enroll";
                response = await client.PostAsync(requestUri, content);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EnrollmentResponse>(resultStr);
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                IEnumerable<string> operationLocation = response.Headers.GetValues(OPERATION_LOCATION_HEADER);
                if (operationLocation.Count() == 1)
                {
                    string operationUrl = operationLocation.First();
                    using (HttpClient client = new HttpClient())
                    {
                        // Construct the request header
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT_HEADER_VALUE));
                        client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                        // Send the request
                        EnrollmentOperationResponse operationResponse;
                        while (true)
                        {
                            response = await client.GetAsync(operationUrl);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                // parse response
                                string resultStr = await response.Content.ReadAsStringAsync();
                                operationResponse = JsonConvert.DeserializeObject<EnrollmentOperationResponse>(resultStr);
                            }
                            else
                            {
                                throw new Exception("Cannot enroll profile: " + response.ReasonPhrase);
                            }

                            if (operationResponse.Status == OPERATION_STATUS_SUCCEEDED)
                            {
                                return operationResponse.ProcessingResult;
                            }
                            else
                            {
                                await Task.Delay(OPERATION_STATUS_UPDATE_DELAY);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Cannot enroll profile: Incorrect server response");
                }
            }
            else
            {
                throw new Exception("Cannot enroll profile: " + response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Gets a user profile from the service asynchronously
        /// </summary>
        /// <param name="profileId">The Id of the profile to get</param>
        /// <returns>The desired profile</returns>
        public async Task<IdentificationProfile> GetProfileAsync(string profileId)
        {
            using (HttpClient client = new HttpClient())
            {
                // Construct the request header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT_HEADER_VALUE));
                client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                // Construct the request URI
                profileId = Uri.EscapeDataString(profileId);
                string requestUri = IDENTIFICATION_PROFILE_URI + "/" + profileId;

                // Send the request
                var response = await client.GetAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    string resultStr = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<IdentificationProfile>(resultStr);
                }
                else
                {
                    throw new Exception("Cannot get profile: " + response.ReasonPhrase);
                }
            }
        }

        /// <summary>
        /// Identify an audio stream
        /// </summary>
        /// <param name="audioStream">The audio stream to identify</param>
        /// <param name="testProfileIds">The list of possible profile IDs to identify from</param>
        /// <returns>The identification response</returns>
        public async Task<IdentificationResponse> IdentiftyAsync(Stream audioStream, string[] testProfileIds)
        {
            HttpResponseMessage response;
            using (HttpClient client = new HttpClient())
            {
                // Construct the request header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(STREAM_CONTENT_HEADER_VALUE));
                client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                // Construct the request body
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString());
                content.Add(new StreamContent(audioStream), "identificationData", "testFile_" + DateTime.Now.ToString());

                // Construct the request URI
                string testProfileIdsString = testProfileIds[0];
                for (int i = 1; i < testProfileIds.Length; i++)
                    testProfileIdsString += "," + testProfileIds[i];
                string requestUri = IDENTIFICATION_URI + "?identificationProfileIds=" + Uri.EscapeDataString(testProfileIdsString);

                // Send the request
                response = await client.PostAsync(requestUri, content);
            }

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IdentificationResponse>(resultStr);
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                IEnumerable<string> operationLocation = response.Headers.GetValues(OPERATION_LOCATION_HEADER);
                if (operationLocation.Count() == 1)
                {
                    string operationUrl = operationLocation.First();
                    using (HttpClient client = new HttpClient())
                    {
                        // Construct the request header
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT_HEADER_VALUE));
                        client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                        // Send the request
                        IdentificationOperationResponse operationResponse;
                        while (true)
                        {
                            response = await client.GetAsync(operationUrl);
                            if (response.StatusCode == HttpStatusCode.OK)
                            {
                                // parse response
                                string resultStr = await response.Content.ReadAsStringAsync();
                                operationResponse = JsonConvert.DeserializeObject<IdentificationOperationResponse>(resultStr);
                            }
                            else
                            {
                                throw new Exception("Cannot perform identification: " + response.ReasonPhrase);
                            }

                            if (operationResponse.Status == OPERATION_STATUS_SUCCEEDED)
                            {
                                return operationResponse.ProcessingResult;
                            }
                            else
                            {
                                await Task.Delay(OPERATION_STATUS_UPDATE_DELAY);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Cannot perform identification: Incorrect server response");
                }
            }
            else
            {
                throw new Exception("Cannot perform identification: " + response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Gets all user profiles from the service asynchronously
        /// </summary>
        /// <returns>A list of all profiles</returns>
        public async Task<List<IdentificationProfile>> GetAllProfilesAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                // Construct the request header
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JSON_CONTENT_HEADER_VALUE));
                client.DefaultRequestHeaders.Add(SUBSCRIPTION_KEY_HEADER, _subscriptionKey);

                // Construct the request URI
                string requestUri = IDENTIFICATION_PROFILE_URI;

                // Send the request
                var response = await client.GetAsync(requestUri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    string resultStr = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<IdentificationProfile>>(resultStr);
                }
                else
                {
                    throw new Exception("Cannot get profiles: " + response.ReasonPhrase);
                }
            }
        }
    }
}
