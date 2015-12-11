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
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net;

namespace Microsoft.ProjectOxford.Speech.SpeakerIdentification
{
    /// <summary>
    /// This class abstracts all the identification service calls
    /// </summary>
    public class SpeechIdServiceClient
    {
        /// <summary>
        /// The Http client used to communicate with the service
        /// </summary>
        private HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// Address of the identification profiles API
        /// </summary>
        private const string _IDENTIFICATION_PROFILE_URI = "https://api.projectoxford.ai/spid/v1.0/identificationProfiles";

        /// <summary>
        /// Address of the identification API
        /// </summary>
        private const string _IDENTIFICATION_URI = "https://api.projectoxford.ai/spid/v1.0/identify";

        /// <summary>
        /// The header for the subscription key
        /// </summary>
        private const string _SUBSCRIPTION_KEY_HEADER = "Ocp-Apim-Subscription-Key";

        /// <summary>
        /// Json content type header value
        /// </summary>
        private const string _JSON_CONTENT_HEADER_VALUE = "application/json";

        /// <summary>
        /// The operation location header field
        /// </summary>
        private const string _OPERATION_LOCATION_HEADER = "Operation-Location";

        /// <summary>
        /// The default resolver.
        /// </summary>
        private static CamelCasePropertyNamesContractResolver s_defaultResolver = new CamelCasePropertyNamesContractResolver();

        /// <summary>
        /// Settings for the Json converter to parse DateTime in the ISO 8601 format
        /// </summary>
        private static JsonSerializerSettings s_jsonDateTimeSettings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = s_defaultResolver
        };

        /// <summary>
        /// Initializes an instance of the service client
        /// </summary>
        /// <param name="subscriptionKey">The subscription key to use</param>
        public SpeechIdServiceClient(string subscriptionKey)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_JSON_CONTENT_HEADER_VALUE));
            _httpClient.DefaultRequestHeaders.Add(_SUBSCRIPTION_KEY_HEADER, subscriptionKey);
        }

        /// <summary>
        /// Create a new profile asynchronously
        /// </summary>
        /// <param name="userData">The user data associated with the profile</param>
        /// <param name="locale">The profile locale</param>
        /// <returns>The response of the profile create request</returns>
        public async Task<CreateProfileResponse> CreateProfileAsync(string locale)
        {

            // Construct the request body
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("locale", locale),
            });

            // Send the request
            HttpResponseMessage response = await _httpClient.PostAsync(_IDENTIFICATION_PROFILE_URI, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<CreateProfileResponse>(resultStr);
            }
            else
            {
                throw new ProfileCreationException(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Enrolls a profile with an audio stream
        /// </summary>
        /// <param name="audioStream">The audio stream to use for enrollment</param>
        /// <param name="profileId">The profile ID to enroll</param>
        /// <param name="retryDelay">The delay to retry polling the Enrollment operation status</param>
        /// <param name="numberOfRetries">The maximum number of retries before throwing an exception</param>
        /// <returns>The enrollment response</returns>
        public async Task<EnrollmentResponse> EnrollAsync(Stream audioStream, string profileId, TimeSpan retryDelay, int numberOfRetries)
        {
            // Construct the request body
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
            content.Add(new StreamContent(audioStream), "enrollmentData", profileId + "_" + DateTime.Now.ToString("u"));

            // Send the request
            profileId = Uri.EscapeDataString(profileId);
            string requestUri = _IDENTIFICATION_PROFILE_URI + "/" + profileId + "/enroll";
            HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EnrollmentResponse>(resultStr);
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                IEnumerable<string> operationLocation = response.Headers.GetValues(_OPERATION_LOCATION_HEADER);
                if (operationLocation.Count() == 1)
                {
                    string operationUrl = operationLocation.First();

                    // Send the request
                    EnrollmentOperationResponse operationResponse;
                    while (numberOfRetries-- > 0)
                    {
                        await Task.Delay(retryDelay);

                        response = await _httpClient.GetAsync(operationUrl);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // parse response
                            string resultStr = await response.Content.ReadAsStringAsync();
                            operationResponse = JsonConvert.DeserializeObject<EnrollmentOperationResponse>(resultStr, s_jsonDateTimeSettings);
                        }
                        else
                        {
                            throw new EnrollmentException(response.ReasonPhrase);
                        }

                        if (operationResponse.Status == OperationStatus.Succeeded)
                        {
                            return operationResponse.ProcessingResult;
                        }
                        else if (operationResponse.Status == OperationStatus.Failed)
                        {
                            throw new EnrollmentException(operationResponse.Message);
                        }
                    }

                    throw new IdentificationException("Polling on operation status timed out");
                }
                else
                {
                    throw new EnrollmentException("Incorrect server response");
                }
            }
            else
            {
                throw new EnrollmentException(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Gets a user profile from the service asynchronously
        /// </summary>
        /// <param name="profileId">The Id of the profile to get</param>
        /// <returns>The desired profile</returns>
        public async Task<IdentificationProfile> GetProfileAsync(string profileId)
        {
            // Construct the request URI
            profileId = Uri.EscapeDataString(profileId);
            string requestUri = _IDENTIFICATION_PROFILE_URI + "/" + profileId;

            // Send the request
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IdentificationProfile>(resultStr, s_jsonDateTimeSettings);
            }
            else
            {
                throw new GetProfileException(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Identify an audio stream
        /// </summary>
        /// <param name="audioStream">The audio stream to identify</param>
        /// <param name="testProfileIds">The list of possible profile IDs to identify from</param>
        /// <param name="retryDelay">The delay to retry polling the Identificaiton operation status</param>
        /// <param name="numberOfRetries">The maximum number of retries before throwing an exception</param>
        /// <returns>The identification response</returns>
        public async Task<IdentificationResponse> IdentiftyAsync(Stream audioStream, string[] testProfileIds, TimeSpan retryDelay, int numberOfRetries)
        {
            // Construct the request body
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
            content.Add(new StreamContent(audioStream), "identificationData", "testFile_" + DateTime.Now.ToString("u"));

            // Construct the request URI
            string testProfileIdsString = testProfileIds[0];
            for (int i = 1; i < testProfileIds.Length; i++)
                testProfileIdsString += "," + testProfileIds[i];
            string requestUri = _IDENTIFICATION_URI + "?identificationProfileIds=" + Uri.EscapeDataString(testProfileIdsString);

            // Send the request
            HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IdentificationResponse>(resultStr);
            }
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                IEnumerable<string> operationLocation = response.Headers.GetValues(_OPERATION_LOCATION_HEADER);
                if (operationLocation.Count() == 1)
                {
                    string operationUrl = operationLocation.First();

                    // Send the request
                    IdentificationOperationResponse operationResponse;
                    while (numberOfRetries-- > 0)
                    {
                        await Task.Delay(retryDelay);
                        response = await _httpClient.GetAsync(operationUrl);
                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            // parse response
                            string resultStr = await response.Content.ReadAsStringAsync();
                            operationResponse = JsonConvert.DeserializeObject<IdentificationOperationResponse>(resultStr, s_jsonDateTimeSettings);
                        }
                        else
                        {
                            throw new IdentificationException(response.ReasonPhrase);
                        }

                        if (operationResponse.Status == OperationStatus.Succeeded)
                        {
                            return operationResponse.ProcessingResult;
                        }
                        else if (operationResponse.Status == OperationStatus.Failed)
                        {
                            throw new IdentificationException(operationResponse.Message);
                        }
                    }

                    throw new IdentificationException("Polling on operation status timed out");
                }
                else
                {
                    throw new IdentificationException("Incorrect server response");
                }
            }
            else
            {
                throw new IdentificationException(response.ReasonPhrase);
            }
        }

        /// <summary>
        /// Gets all user profiles from the service asynchronously
        /// </summary>
        /// <returns>A list of all profiles</returns>
        public async Task<List<IdentificationProfile>> GetAllProfilesAsync()
        {
            // Construct the request URI
            string requestUri = _IDENTIFICATION_PROFILE_URI;

            // Send the request
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                //parse response
                string resultStr = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<IdentificationProfile>>(resultStr, s_jsonDateTimeSettings);
            }
            else
            {
                throw new GetProfileException(response.ReasonPhrase);
            }
        }
    }
}
