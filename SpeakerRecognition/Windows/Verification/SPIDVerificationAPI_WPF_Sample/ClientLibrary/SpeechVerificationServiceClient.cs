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

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Speech.SpeakerVerification
{
    /// <summary>
    /// A service client class to perform all the verification service calls.
    /// </summary>
    public class SpeechVerificationServiceClient
    {
        /// <summary>
        /// Address of the verification profiles API
        /// </summary>
        private const string _BASE_URI = "https://api.projectoxford.ai/spid/v1.0/verificationProfiles";

        /// <summary>
        /// Address of the verification API
        /// </summary>
        private const string _VERIFY_ENDPOINT = "https://api.projectoxford.ai/spid/v1.0/verify";

        /// <summary>
        /// Address of the verification phrases API
        /// </summary>
        private const string _PHRASES_ENDPOINT = "https://api.projectoxford.ai/spid/v1.0/verificationPhrases?locale=";

        /// <summary>
        /// The header for the subscription key
        /// </summary>
        private const string _OCP_SUBSCRIPTION_KEY_HEADER = "Ocp-Apim-Subscription-Key";

        /// <summary>
        /// Json content type header value
        /// </summary>
        private const string _JSON_HEADER = "application/json";

        /// <summary>
        /// The locale parameter
        /// </summary>
        private const string _LOCALE_PARAM = "locale";

        /// <summary>
        /// The Http client used to communicate with the service
        /// </summary>
        private HttpClient _DefaultHttpClient = new HttpClient();

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
        /// Creates a new service client using a subscription key
        /// </summary>
        /// <param name="SubscriptionKey">The subscription key</param>
        public SpeechVerificationServiceClient(string SubscriptionKey)
        {
            _DefaultHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_JSON_HEADER));
            _DefaultHttpClient.DefaultRequestHeaders.Add(_OCP_SUBSCRIPTION_KEY_HEADER, SubscriptionKey);
        }

        /// <summary>
        /// Creates a new profile asynchronously
        /// </summary>
        /// <param name="locale">The profile locale</param>
        /// <returns>SpeakerProfile encapsulating the profile repsonse</returns>
        public async Task<SpeakerProfile> CreateProfileAsync(string locale)
        {
            var content = new FormUrlEncodedContent(new[]
             {
                        new KeyValuePair<string, string>(_LOCALE_PARAM, locale),
                 });
            var result = await _DefaultHttpClient.PostAsync(_BASE_URI, content);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //parse response
                string resultStr = await result.Content.ReadAsStringAsync();
                var obj = JsonConvert.DeserializeObject<SpeakerProfile>(resultStr, s_jsonDateTimeSettings);
                return obj;
            }
            else
            {
                throw new ProfileCreationException(result.StatusCode.ToString());
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
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
            content.Add(new StreamContent(audioStream), "verificationData", speakerId + "_" + DateTime.Now.ToString("u"));
            try
            {
                speakerId = Uri.EscapeDataString(speakerId);
                var requestURL = _VERIFY_ENDPOINT + "?verificationProfileId=" + speakerId;
                var result = await _DefaultHttpClient.PostAsync(requestURL, content);
                string resultStr = await result.Content.ReadAsStringAsync();
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //parse response
                    VerificationResult response = JsonConvert.DeserializeObject<VerificationResult>(resultStr);
                    return response;
                }
                else
                {
                    EnrollmentVerificationError errorResponse = JsonConvert.DeserializeObject<EnrollmentVerificationError>(resultStr);
                    if (errorResponse.Error != null)
                        throw new VerificationException(errorResponse.Error.Message);
                    else
                        throw new VerificationException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
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
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
            content.Add(new StreamContent(audioStream), "enrollmentData", speakerId + "_" + DateTime.Now.ToString("u"));
            try
            {
                speakerId = Uri.EscapeDataString(speakerId);
                var requestUrl = _BASE_URI + "/" + speakerId + "/enroll";
                var result = await _DefaultHttpClient.PostAsync(requestUrl, content);
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
                        throw new EnrollmentException(errorResponse.Error.Message);
                    else
                        throw new EnrollmentException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }

        }

        /// <summary>
        /// Gets a list of all available phrases for enrollments
        /// </summary>
        /// <param name="locale">The locale of the pharases</param>
        /// <returns></returns>
        public async Task<List<PhraseResponse>> GetAllAvailablePhrases(string locale)
        {
            try
            {
                locale = Uri.EscapeDataString(locale);
                var requestUrl = _PHRASES_ENDPOINT + locale;
                var result = await _DefaultHttpClient.GetAsync(requestUrl);
                string resultStr = await result.Content.ReadAsStringAsync();
                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //parse response
                    List<PhraseResponse> response = JsonConvert.DeserializeObject<List<PhraseResponse>>(resultStr);
                    return response;
                }
                else
                {
                    throw new PhrasesException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }

        }
        /// <summary>
        /// Resets a new profile asynchronously
        /// </summary>
        /// <param name="speakerId">The speaker Id</param>
        /// <returns>SpeakerProfile encapsulating the profile repsonse</returns>
        public async Task ResetProfileAsync(string speakerId)
        {
            speakerId = Uri.EscapeDataString(speakerId);
            var result = await _DefaultHttpClient.PostAsync(_BASE_URI + "/" + speakerId + "/reset", null);
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ResetProfileException(result.StatusCode.ToString());
            }
        }
    }
}
