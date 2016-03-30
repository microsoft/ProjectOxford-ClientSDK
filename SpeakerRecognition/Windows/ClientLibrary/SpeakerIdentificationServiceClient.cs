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
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.SpeakerRecognition
{
    /// <summary>
    /// This class abstracts all the identification service calls
    /// </summary>
    public class SpeakerIdentificationServiceClient : ISpeakerIdentificationServiceClient
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
        public SpeakerIdentificationServiceClient(string subscriptionKey)
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_JSON_CONTENT_HEADER_VALUE));
            _httpClient.DefaultRequestHeaders.Add(_SUBSCRIPTION_KEY_HEADER, subscriptionKey);
        }

        /// <summary>
        /// Creates a new profile asynchronously
        /// </summary>
        /// <param name="locale">The speaker profile locale</param>
        /// <exception cref="CreateProfileException">Thrown on cases of internal server error or an invalid locale</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>The profile object encapsulating the response object</returns>
        public async Task<CreateProfileResponse> CreateProfileAsync(string locale)
        {
            try
            {
                // Construct the request body
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("locale", locale)
                });

                // Send the request
                HttpResponseMessage response = await _httpClient.PostAsync(_IDENTIFICATION_PROFILE_URI, content).ConfigureAwait(false);
                string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    return JsonConvert.DeserializeObject<CreateProfileResponse>(resultStr);
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new CreateProfileException(errorResponse.Error.Message);
                    else
                        throw new CreateProfileException(response.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Retrieves a speaker profile from the service asynchronously
        /// </summary>
        /// <param name="id">The ID of the speaker profile to get</param>
        /// <exception cref="GetProfileException">Thrown in cases of invalid ID or an internal server error</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>The requested speaker profile</returns>
        public async Task<Profile> GetProfileAsync(Guid id)
        {
            try
            {
                // Construct the request URI
                string requestUri = _IDENTIFICATION_PROFILE_URI + "/" + id.ToString("D");

                // Send the request
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);
                string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    return JsonConvert.DeserializeObject<Profile>(resultStr, s_jsonDateTimeSettings);
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new GetProfileException(errorResponse.Error.Message);
                    else
                        throw new GetProfileException(response.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Identifies an audio stream asynchronously
        /// </summary>
        /// <param name="audioStream">The audio stream to identify</param>
        /// <param name="ids">The list of possible speaker profile IDs to identify from</param>
        /// <exception cref="IdentificationException">Thrown on cases of internal server error, invalid IDs or wrong audio format</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>An object encapsulating the The Url that can be used to query the identification operation status</returns>
        public async Task<OperationLocation> IdentifyAsync(Stream audioStream, Guid[] ids)
        {
            try
            {
                // Construct the request URI
                string testProfileIds = string.Join(",", ids.Select(id => id.ToString("D")));
                string requestUri = _IDENTIFICATION_URI + "?identificationProfileIds=" + testProfileIds;
                OperationLocation location = await OperationAsync<IdentificationException>(audioStream, requestUri).ConfigureAwait(false);
                return location;
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Enrolls a speaker profile from an audio stream asynchronously
        /// </summary>
        /// <param name="audioStream">The audio stream to use for enrollment</param>
        /// <param name="id">The speaker profile ID to enroll</param>
        /// <exception cref="EnrollmentException">Thrown in case of invalid audio format, internal server error or an invalid ID</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>The operation location object encapsulating the Url that can be used to check the status of the operation</returns>
        public async Task<OperationLocation> EnrollAsync(Stream audioStream, Guid id)
        {
            try
            {
                // Send the request
                string requestUri = _IDENTIFICATION_PROFILE_URI + "/" + id.ToString("D") + "/enroll";
                OperationLocation location = await OperationAsync<EnrollmentException>(audioStream, requestUri).ConfigureAwait(false);
                return location;
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Gets the identification operation status or result asynchronously
        /// </summary>
        /// <param name="location">The location returned upon calling the identification operation</param>
        /// <exception cref="IdentificationException">Thrown in case of internal server error or an url</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>The identification object encapsulating the result</returns>
        public async Task<IdentificationOperation> CheckIdentificationStatusAsync(OperationLocation location)
        {
            try
            {
                IdentificationOperation operation = await CheckStatusAsync<IdentificationOperation, IdentificationException>(location.Url).ConfigureAwait(false);
                return operation;
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Gets the enrollment operation status or result asynchronously
        /// </summary>
        /// <param name="location">The location returned upon enrollment</param>
        /// <exception cref="EnrollmentException">Thrown in case of internal server error or an invalid url</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>The enrollment object encapsulating the result</returns>
        public async Task<EnrollmentOperation> CheckEnrollmentStatusAsync(OperationLocation location)
        {
            try
            {
                EnrollmentOperation operation = await CheckStatusAsync<EnrollmentOperation, EnrollmentException>(location.Url).ConfigureAwait(false);
                return operation;
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }
 
        /// <summary>
        /// A helper method that's used to check the status for an identification/enrollment request
        /// </summary>
        /// <typeparam name="T">The type of request identification/enrollment</typeparam>
        /// <typeparam name="E">The type of exception</typeparam>
        /// <param name="url">The Url of the operation</param>
        /// <returns>An operation object encoding the result</returns>
        private async Task<T> CheckStatusAsync<T, E>(string url) where T : Operation where E : Exception
        {
            var response = await _httpClient.GetAsync(url);
            string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                // parse response
                T operationResponse = JsonConvert.DeserializeObject<T>(resultStr, s_jsonDateTimeSettings);
                return operationResponse;
            }
            else
            {
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                if (errorResponse.Error != null)
                    throw CreateExceptionFromType<E>(errorResponse.Error.Message);
                else
                    throw CreateExceptionFromType<E>(response.StatusCode.ToString());
            }
        }

        /// <summary>
        /// A helper method that's used to create an enrollment/identification request
        /// </summary>
        /// <typeparam name="E">The type of exception</typeparam>
        /// <param name="audioStream">The audio stream to enroll/identify from</param>
        /// <param name="requestUri">The Http endpoint for identification/enrollment</param>
        /// <returns>An operation object encoding the operation Url</returns>
        private async Task<OperationLocation> OperationAsync<E>(Stream audioStream, string requestUri) where E :Exception
        {
            var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString("u"));
            content.Add(new StreamContent(audioStream), "Data", "testFile_" + DateTime.Now.ToString("u"));
            HttpResponseMessage response = await _httpClient.PostAsync(requestUri, content).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.Accepted)
            {
                IEnumerable<string> operationLocation = response.Headers.GetValues(_OPERATION_LOCATION_HEADER);
                if (operationLocation.Count() == 1)
                {
                    string operationUrl = operationLocation.First();
                    OperationLocation location = new OperationLocation();
                    location.Url = operationUrl;
                    return location;
                }
                else
                {
                    throw CreateExceptionFromType<E>("Incorrect server response");
                }
            }
            else
            {
                string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                if (errorResponse.Error != null)
                    throw CreateExceptionFromType<E>(errorResponse.Error.Message);
                else
                    throw CreateExceptionFromType<E>(response.StatusCode.ToString());
            }
        }

        /// <summary>
        /// Gets all speaker profiles from the service asynchronously
        /// </summary>
        /// <exception cref="GetProfileException">Thrown on internal server error</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        /// <returns>An array containing a list of all profiles</returns>
        public async Task<Profile[]> GetProfilesAsync()
        {
            try
            {
                // Construct the request URI
                string requestUri = _IDENTIFICATION_PROFILE_URI;

                // Send the request
                HttpResponseMessage response = await _httpClient.GetAsync(requestUri).ConfigureAwait(false);
                string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //parse response
                    List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(resultStr, s_jsonDateTimeSettings);
                    if (profiles != null)
                    {
                        return profiles.ToArray();
                    }
                    return null;
                }
                else
                {
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new GetProfileException(errorResponse.Error.Message);
                    else
                        throw new GetProfileException(response.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Deletes all enrollments associated with the given speaker identification profile permanently from the service asynchronously
        /// </summary>
        /// <param name="id">The speaker ID</param>
        /// <exception cref="ResetEnrollmentsException">Thrown in case of internal server error or an invalid ID</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        public async Task ResetEnrollmentsAsync(Guid id)
        {
            try
            {
                string requestUri = _IDENTIFICATION_PROFILE_URI + "/" + id.ToString("D") + "/reset";
                HttpResponseMessage response = await _httpClient.PostAsync(requestUri, null).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new ResetEnrollmentsException(errorResponse.Error.Message);
                    else
                        throw new ResetEnrollmentsException(response.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// Deletes a given profile asynchronously
        /// </summary>
        /// <param name="id">The ID of the speaker profile to be deleted</param>
        /// <exception cref="DeleteProfileException">Thrown on cases of internal server error on an invalid ID</exception>
        /// <exception cref="TimeoutException">Thrown in case the connection timed out</exception>
        public async Task DeleteProfileAsync(Guid id)
        {
            try
            {
                string requestUri = _IDENTIFICATION_PROFILE_URI + "/" + id.ToString("D");
                HttpResponseMessage response = await _httpClient.DeleteAsync(requestUri).ConfigureAwait(false);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    string resultStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    ErrorResponse errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(resultStr);
                    if (errorResponse.Error != null)
                        throw new DeleteProfileException(errorResponse.Error.Message);
                    else
                        throw new DeleteProfileException(response.StatusCode.ToString());
                }
            }
            catch (TaskCanceledException exception)
            {
                throw new TimeoutException("Connection timed out: " + exception.Message);
            }
        }

        /// <summary>
        /// A helper method used to generate an exception from generic types
        /// </summary>
        /// <typeparam name="E">The exception type</typeparam>
        /// <param name="msg">The exception message</param>
        /// <returns>An exception derived from the given type with the specified message as supplied by the parameter</returns>
        private E CreateExceptionFromType<E>(string msg) where E: Exception
        {
            return (E)Activator.CreateInstance(typeof(E), msg);
        }
    }
}
