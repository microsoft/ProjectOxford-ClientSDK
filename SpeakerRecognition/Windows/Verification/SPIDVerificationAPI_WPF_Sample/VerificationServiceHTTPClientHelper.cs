// 
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
// 

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace SPIDVerificationAPI_WPF_Sample
{
    /// <summary>
    /// A helper class to perform all the verification service calls.
    /// </summary>
    public class VerificationServiceHttpClientHelper
    {
        private const string BASE_URI = "https://api.projectoxford.ai/spid/v1.0/verificationProfiles";
        private const string VERIFY_ENDPOINT = "https://api.projectoxford.ai/spid/v1.0/verify";
        private const string PHRASES_ENDPOINT = "https://api.projectoxford.ai/spid/v1.0/verificationPhrases?locale=";
        private const string _OCP_SUBSCRIPTION_KEY_HEADER = "Ocp-Apim-Subscription-Key";
        private const string _OCTET_STREAM_HEADER = "application/octet-stream";
        private const string _JSON_HEADER = "application/json";
        private const string _LOCALE_PARAM = "locale";
        /// <summary>
        /// The subscription key used for accessing Oxford SPID APIs
        /// </summary>
        public string SubscriptionKey { get; set; }
        /// <summary>
        /// Creates a new helper
        /// </summary>
        public VerificationServiceHttpClientHelper()
        {
        }
        /// <summary>
        /// Creates a new profile asynchronously
        /// </summary>
        /// <param name="locale">The profile locale</param>
        /// <returns>SpeakerProfile encapsulating the profile repsonse</returns>
        public async Task<SpeakerProfile> CreateProfileAsync(string locale)
        {
            using (var client = new HttpClient())
            {
                // Request headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_JSON_HEADER));
                client.DefaultRequestHeaders.Add(_OCP_SUBSCRIPTION_KEY_HEADER, SubscriptionKey);
                var content = new FormUrlEncodedContent(new[]
                 {
                        new KeyValuePair<string, string>(_LOCALE_PARAM, locale),
                 });
                var result = await client.PostAsync(BASE_URI, content);
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //parse response
                    string resultStr = await result.Content.ReadAsStringAsync();
                    var obj = JsonConvert.DeserializeObject<SpeakerProfile>(resultStr);
                    return obj;
                }
                else
                {
                    throw new Exception("Cannot Create Profile: " + result.StatusCode.ToString());
                }
            }
        }
        /// <summary>
        /// Verifies a given speaker using the speaker Id and audio stream
        /// </summary>
        /// <param name="audioStream">The stream of audio to be verified</param>
        /// <param name="speakerId">The speakerId</param>
        /// <returns>VerificationResult encapsulating the verification result</returns>
        public async Task<VerificationResult> VerifyAsync(Stream audioStream, string speakerId)
        {
            using (var client = new HttpClient())
            {
                // Request headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_OCTET_STREAM_HEADER));
                client.DefaultRequestHeaders.Add(_OCP_SUBSCRIPTION_KEY_HEADER, SubscriptionKey);
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString());
                content.Add(new StreamContent(audioStream), "verificationData", speakerId + "_" + DateTime.Now.ToString());
                try
                {
                    speakerId = Uri.EscapeDataString(speakerId);
                    var requestURL = VERIFY_ENDPOINT + "?verificationProfileId=" + speakerId;
                    var result = await client.PostAsync(requestURL, content);
                    string resultStr = await result.Content.ReadAsStringAsync();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //parse response
                        VerificationResult response = JsonConvert.DeserializeObject<VerificationResult>(resultStr);
                        return response;
                    }
                    else
                    {
                        throw new Exception("Cannot Verify Speaker: " + result.StatusCode.ToString());
                    }
                }
                catch (TaskCanceledException exception)
                {
                    throw new TimeoutException("Connection timed out: " + exception.Message);
                }
            }
        }
        /// <summary>
        /// Enroll a new stream asynchronously for a given speaker
        /// </summary>
        /// <param name="audioStream">The stream to enroll</param>
        /// <param name="speakerId">The speaker profile speaker Id</param>
        /// <returns>EnrollmentResponse encapsulating the enrollment response</returns>
        public async Task<EnrollmentResponse> EnrollStreamAsync(Stream audioStream, string speakerId)
        {
            using (var client = new HttpClient())
            {
                // Request headers
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_OCTET_STREAM_HEADER));
                client.DefaultRequestHeaders.Add(_OCP_SUBSCRIPTION_KEY_HEADER, SubscriptionKey);
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString());
                content.Add(new StreamContent(audioStream), "enrollmentData", speakerId + "_" + DateTime.Now.ToString());
                try
                {
                    speakerId = Uri.EscapeDataString(speakerId);
                    var requestUrl = BASE_URI + "/" + speakerId + "/enroll";
                    var result = await client.PostAsync(requestUrl, content);
                    string resultStr = await result.Content.ReadAsStringAsync();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //parse response
                        EnrollmentResponse response = JsonConvert.DeserializeObject<EnrollmentResponse>(resultStr);
                        return response;
                    }
                    else
                    {
                        EnrollmentVerificationError errorResponse = JsonConvert.DeserializeObject<EnrollmentVerificationError>(resultStr);
                        if (errorResponse.Error != null)
                            throw new Exception("Cannot enroll audio: " + errorResponse.Error.Message);
                        else
                            throw new Exception("Cannot enroll audio: " + result.StatusCode.ToString());
                    }
                }
                catch (TaskCanceledException exception)
                {
                    throw new TimeoutException("Connection timed out: " + exception.Message);
                }
            }
        }
        public async Task<List<PhraseResponse>> GetAllAvailablePhrases(string locale)
        {
            using (var client = new HttpClient())
            {
                // Request headers
                client.DefaultRequestHeaders.Add(_OCP_SUBSCRIPTION_KEY_HEADER, SubscriptionKey);
                try
                {
                    locale = Uri.EscapeDataString(locale);
                    var requestUrl = PHRASES_ENDPOINT + locale;
                    var result = await client.GetAsync(requestUrl);
                    string resultStr = await result.Content.ReadAsStringAsync();
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //parse response
                        List<PhraseResponse> response = JsonConvert.DeserializeObject<List<PhraseResponse>>(resultStr);
                        return response;
                    }
                    else
                    {
                        throw new Exception("Cannot retrieve phrases: " + result.StatusCode.ToString());
                    }
                }
                catch (TaskCanceledException exception)
                {
                    throw new TimeoutException("Connection timed out: " + exception.Message);
                }
            }
        }
    }
}
