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

using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Verification;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.SpeakerRecognition
{
    /// <summary>
    /// A service client class to perform all the verification service calls.
    /// </summary>
    public class SpeakerVerificationServiceClient : ISpeakerVerificationServiceClient
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
        public SpeakerVerificationServiceClient(string SubscriptionKey)
        {
            _DefaultHttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_JSON_HEADER));
            _DefaultHttpClient.DefaultRequestHeaders.Add(_OCP_SUBSCRIPTION_KEY_HEADER, SubscriptionKey);
        }

        /// <summary>
        /// Creates a new speaker profile asynchronously
        /// </summary>
        /// <param name="locale">The speaker profile locale</param>
        /// <exception cref="CreateProfileException">Thrown in case of internal server error or an invalid locale</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>SpeakerProfile encapsulating the speaker profile response</returns>
        public async Task<CreateProfileResponse> CreateProfileAsync(string locale)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>(_LOCALE_PARAM, locale)
                });

                var result = await _DefaultHttpClient.PostAsync(_BASE_URI, content).ConfigureAwait(false);
                string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    return JsonConvert.DeserializeObject<CreateProfileResponse>(resultStr, s_jsonDateTimeSettings);
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new CreateProfileException(errorResponse.Error.Message);
                    else
                        throw new CreateProfileException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Verifies a given speaker using the speaker ID and audio stream
        /// </summary>
        /// <param name="audioStream">The stream of audio to be verified</param>
        /// <param name="id">The speaker ID</param>
        /// <exception cref="VerificationException">Thrown in case of invalid ID, invalid audio format or internal server error</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>Verification object encapsulating the verification result</returns>
        public async Task<Verification> VerifyAsync(Stream audioStream, Guid id)
        {
            try
            {
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
                content.Add(new StreamContent(audioStream), "verificationData", id.ToString("D") + "_" + DateTime.Now.ToString("u"));

                var requestUrl = _VERIFY_ENDPOINT + "?verificationProfileId=" + id.ToString("D");
                var result = await _DefaultHttpClient.PostAsync(requestUrl, content).ConfigureAwait(false);
                string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    Verification response = JsonConvert.DeserializeObject<Verification>(resultStr);
                    return response;
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
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
        /// Enrolls a new stream asynchronously for a given speaker
        /// </summary>
        /// <param name="audioStream">The stream to enroll the speaker profile from</param>
        /// <param name="id">The speaker profile speaker ID</param>
        /// <exception cref="EnrollmentException">Thrown in case of internal server error, wrong ID or an invalid audio format</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>An enrollment object encapsulating the enrollment response</returns>
        public async Task<Enrollment> EnrollAsync(Stream audioStream, Guid id)
        {
            try
            {
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
                content.Add(new StreamContent(audioStream), "enrollmentData", id.ToString("D") + "_" + DateTime.Now.ToString("u"));

                var requestUrl = _BASE_URI + "/" + id.ToString("D") + "/enroll";
                var result = await _DefaultHttpClient.PostAsync(requestUrl, content).ConfigureAwait(false);
                string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    Enrollment response = JsonConvert.DeserializeObject<Enrollment>(resultStr);
                    return response;
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
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
        /// <exception cref="PhrasesException">Thrown in case of invalid locale or internal server error</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>An array containing a list of all verification phrases</returns>
        public async Task<VerificationPhrase[]> GetPhrasesAsync(string locale)
        {
            try
            {
                locale = Uri.EscapeDataString(locale);
                var requestUrl = _PHRASES_ENDPOINT + locale;
                var result = await _DefaultHttpClient.GetAsync(requestUrl).ConfigureAwait(false);
                string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    List<VerificationPhrase> response = JsonConvert.DeserializeObject<List<VerificationPhrase>>(resultStr);
                    if(response != null)
                    {
                        return response.ToArray();
                    }
                    return null;
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new PhrasesException(errorResponse.Error.Message);
                    else
                        throw new PhrasesException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Deletes all enrollments associated with the given speaker verification profile permanently from the service asynchronously
        /// </summary>
        /// <param name="id">The speaker ID</param>
        /// <exception cref="ResetEnrollmentsException">Thrown in case of invalid ID, failure to reset the profile or an internal server error</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        public async Task ResetEnrollmentsAsync(Guid id)
        {
            try
            {
                string requestUri = _BASE_URI + "/" + id.ToString("D") + "/reset";
                var result = await _DefaultHttpClient.PostAsync(requestUri, null).ConfigureAwait(false);

                if (result.StatusCode != HttpStatusCode.OK)
                {
                    string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new ResetEnrollmentsException(errorResponse.Error.Message);
                    else
                        throw new ResetEnrollmentsException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Deletes a given speaker profile asynchronously
        /// </summary>
        /// <param name="id">The ID of the speaker profile to be deleted</param>
        /// <exception cref="DeleteProfileException">Thrown in case of internal server error, an invalid ID or failure to delete the profile</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        public async Task DeleteProfileAsync(Guid id)
        {
            try
            {
                string requestUri = _BASE_URI + "/" + id.ToString("D");
                var result = await _DefaultHttpClient.DeleteAsync(requestUri).ConfigureAwait(false);
                if (result.StatusCode != HttpStatusCode.OK)
                {
                    string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new DeleteProfileException(errorResponse.Error.Message);
                    else
                        throw new DeleteProfileException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Retrieves a given speaker profile as specified by the id param
        /// </summary>
        /// <param name="id">The speaker profile ID</param>
        /// <exception cref="GetProfileException">Thrown in case of internal server error or an invalid ID</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>The requested speaker profile</returns>
        public async Task<Profile> GetProfileAsync(Guid id)
        {
            try
            {
                string requestUri = _BASE_URI + "/" + id.ToString("D");
                var result = await _DefaultHttpClient.GetAsync(requestUri).ConfigureAwait(false);
                string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    var obj = JsonConvert.DeserializeObject<Profile>(resultStr, s_jsonDateTimeSettings);
                    return obj;
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new GetProfileException(errorResponse.Error.Message);
                    else
                        throw new GetProfileException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Retrieves all available speaker profiles
        /// </summary>
        /// <exception cref="GetProfileException">Thrown in case of internal server error</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>An array containing all speaker profiles</returns>
        public async Task<Profile[]> GetProfilesAsync()
        {
            try
            {
                var result = await _DefaultHttpClient.GetAsync(_BASE_URI).ConfigureAwait(false);
                string resultStr = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (result.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    List<Profile> response = JsonConvert.DeserializeObject<List<Profile>>(resultStr);
                    if (response != null)
                    {
                        return response.ToArray();
                    }
                    return null;
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new GetProfileException(errorResponse.Error.Message);
                    else
                        throw new GetProfileException(result.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }
    }
}