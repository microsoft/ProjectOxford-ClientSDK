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

using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Face.Contract;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.ProjectOxford.Face
{


    /// <summary>
    /// The face service client proxy implementation.
    /// </summary>
    public class FaceServiceClient : IFaceServiceClient
    {
        #region private members

        /// <summary>
        /// The service host.
        /// </summary>
        private const string ServiceHost = "https://api.projectoxford.ai/face/v0";

        /// <summary>
        /// The json header
        /// </summary>
        private const string JsonHeader = "application/json";

        /// <summary>
        /// The subscription key name.
        /// </summary>
        private const string SubscriptionKeyName = "subscription-key";

        /// <summary>
        /// The detection.
        /// </summary>
        private const string DetectionsQuery = "detections";

        /// <summary>
        /// The verification.
        /// </summary>
        private const string VerificationsQuery = "verifications";

        /// <summary>
        /// The training query.
        /// </summary>
        private const string TrainingQuery = "training";

        /// <summary>
        /// The person groups.
        /// </summary>
        private const string PersonGroupsQuery = "persongroups";

        /// <summary>
        /// The persons.
        /// </summary>
        private const string PersonsQuery = "persons";

        /// <summary>
        /// The faces query string.
        /// </summary>
        private const string FacesQuery = "faces";

        /// <summary>
        /// The identifications.
        /// </summary>
        private const string IdentificationsQuery = "identifications";

        /// <summary>
        /// The subscription key.
        /// </summary>
        private string _subscriptionKey;

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

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public FaceServiceClient(string subscriptionKey)
        {
            this._subscriptionKey = subscriptionKey;
        }

        #region IFaceServiceClient implementations

        /// <summary>
        /// Detects an URL asynchronously.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="analyzesFaceLandmarks">If set to <c>true</c> [analyzes face landmarks].</param>
        /// <param name="analyzesAge">If set to <c>true</c> [analyzes age].</param>
        /// <param name="analyzesGender">If set to <c>true</c> [analyzes gender].</param>
        /// <param name="analyzesHeadPose">If set to <c>true</c> [analyzes head pose].</param>
        /// <returns>The detected faces.</returns>
        public async Task<Microsoft.ProjectOxford.Face.Contract.Face[]> DetectAsync(string url, bool analyzesFaceLandmarks = false, bool analyzesAge = false, bool analyzesGender = false, bool analyzesHeadPose = false)
        {
            var requestUrl = string.Format(
                "{0}/{1}?analyzesFaceLandmarks={2}&analyzesAge={3}&analyzesGender={4}&analyzesHeadPose={5}&{6}={7}",
                ServiceHost,
                DetectionsQuery,
                analyzesFaceLandmarks,
                analyzesAge,
                analyzesGender,
                analyzesHeadPose,
                SubscriptionKeyName,
                this._subscriptionKey);

            return await this.SendRequestAsync<object, Microsoft.ProjectOxford.Face.Contract.Face[]>(HttpMethod.Post, requestUrl, new { url = url });
        }

        /// <summary>
        /// Detects an image asynchronously.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="analyzesFaceLandmarks">If set to <c>true</c> [analyzes face landmarks].</param>
        /// <param name="analyzesAge">If set to <c>true</c> [analyzes age].</param>
        /// <param name="analyzesGender">If set to <c>true</c> [analyzes gender].</param>
        /// <param name="analyzesHeadPose">If set to <c>true</c> [analyzes head pose].</param>
        /// <returns>The detected faces.</returns>
        public async Task<Microsoft.ProjectOxford.Face.Contract.Face[]> DetectAsync(Stream imageStream, bool analyzesFaceLandmarks = false, bool analyzesAge = false, bool analyzesGender = false, bool analyzesHeadPose = false)
        {
            var requestUrl = string.Format(
                "{0}/{1}?analyzesFaceLandmarks={2}&analyzesAge={3}&analyzesGender={4}&analyzesHeadPose={5}&{6}={7}",
                ServiceHost,
                DetectionsQuery,
                analyzesFaceLandmarks,
                analyzesAge,
                analyzesGender,
                analyzesHeadPose,
                SubscriptionKeyName,
                this._subscriptionKey);

            return await this.SendRequestAsync<Stream, Microsoft.ProjectOxford.Face.Contract.Face[]>(HttpMethod.Post, requestUrl, imageStream);
        }

        /// <summary>
        /// Verifies whether the specified two faces belong to the same person asynchronously.
        /// </summary>
        /// <param name="faceId1">The face id 1.</param>
        /// <param name="faceId2">The face id 2.</param>
        /// <returns>The verification result.</returns>
        public async Task<VerifyResult> VerifyAsync(Guid faceId1, Guid faceId2)
        {
            var requestUrl = string.Format("{0}/{1}?{2}={3}", ServiceHost, VerificationsQuery, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, VerifyResult>(HttpMethod.Post, requestUrl, new { faceId1 = faceId1, faceId2 = faceId2 });
        }

        /// <summary>
        /// Identities the faces in a given person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="faceIds">The face ids.</param>
        /// <param name="maxNumOfCandidatesReturned">The maximum number of candidates returned for each face.</param>
        /// <returns>The identification results</returns>
        public async Task<IdentifyResult[]> IdentifyAsync(string personGroupId, Guid[] faceIds, int maxNumOfCandidatesReturned = 1)
        {
            var requestUrl = string.Format("{0}/{1}?{2}={3}", ServiceHost, IdentificationsQuery, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, IdentifyResult[]>(HttpMethod.Post, requestUrl, new { personGroupId = personGroupId, faceIds = faceIds, maxNumOfCandidatesReturned = maxNumOfCandidatesReturned });
        }

        /// <summary>
        /// Creates the person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>Task object.</returns>
        public async Task CreatePersonGroupAsync(string personGroupId, string name, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}?{3}={4}", ServiceHost, PersonGroupsQuery, personGroupId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(HttpMethod.Put, requestUrl, new { name = name, userData = userData });
        }

        /// <summary>
        /// Gets a person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>The person group entity.</returns>
        public async Task<PersonGroup> GetPersonGroupAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}?{3}={4}", ServiceHost, PersonGroupsQuery, personGroupId, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, PersonGroup>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Updates a person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>Task object.</returns>
        public async Task UpdatePersonGroupAsync(string personGroupId, string name, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}?{3}={4}", ServiceHost, PersonGroupsQuery, personGroupId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(new HttpMethod("PATCH"), requestUrl, new { name = name, userData = userData });
        }

        /// <summary>
        /// Deletes a person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>Task object.</returns>
        public async Task DeletePersonGroupAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}?{3}={4}", ServiceHost, PersonGroupsQuery, personGroupId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Gets all person groups asynchronously.
        /// </summary>
        /// <returns>Person group entity array.</returns>
        public async Task<PersonGroup[]> GetPersonGroupsAsync()
        {
            var requestUrl = string.Format(
                "{0}/{1}?{2}={3}",
                ServiceHost,
                PersonGroupsQuery,
                SubscriptionKeyName,
                this._subscriptionKey);

            return await this.SendRequestAsync<object, PersonGroup[]>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Trains the person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>Task object.</returns>
        public async Task TrainPersonGroupAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}?{4}={5}", ServiceHost, PersonGroupsQuery, personGroupId, TrainingQuery, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(HttpMethod.Post, requestUrl, null);
        }

        /// <summary>
        /// Gets person group training status asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>The person group training status.</returns>
        public async Task<TrainingStatus> GetPersonGroupTrainingStatusAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}?{4}={5}", ServiceHost, PersonGroupsQuery, personGroupId, TrainingQuery, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, TrainingStatus>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Creates a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="faceIds">The face ids.</param>
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>The CreatePersonResult entity.</returns>
        public async Task<CreatePersonResult> CreatePersonAsync(string personGroupId, Guid[] faceIds, string name, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}?{4}={5}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, CreatePersonResult>(HttpMethod.Post, requestUrl, new { faceIds = faceIds, name = name, userData = userData });
        }

        /// <summary>
        /// Gets a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <returns>The person entity.</returns>
        public async Task<Person> GetPersonAsync(string personGroupId, Guid personId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}?{5}={6}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, Person>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Updates a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="faceIds">The face ids.</param>
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>Task object.</returns>
        public async Task UpdatePersonAsync(string personGroupId, Guid personId, Guid[] faceIds, string name, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}?{5}={6}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(new HttpMethod("PATCH"), requestUrl, new { faceIds = faceIds, name = name, userData = userData });
        }

        /// <summary>
        /// Deletes a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <returns>Task object.</returns>
        public async Task DeletePersonAsync(string personGroupId, Guid personId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}?{5}={6}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Gets all persons inside a person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>
        /// The person entity array.
        /// </returns>
        public async Task<Person[]> GetPersonsAsync(string personGroupId)
        {
            var requestUrl = string.Format(
                "{0}/{1}/{2}/{3}?{4}={5}",
                ServiceHost,
                PersonGroupsQuery,
                personGroupId,
                PersonsQuery,
                SubscriptionKeyName,
                this._subscriptionKey);

            return await this.SendRequestAsync<object, Person[]>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Adds a face to a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="faceId">The face id.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task AddPersonFaceAsync(string personGroupId, Guid personId, Guid faceId, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}?{7}={8}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, FacesQuery, faceId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(HttpMethod.Put, requestUrl, new { userData = userData });
        }

        /// <summary>
        /// Gets a face of a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="faceId">The face id.</param>
        /// <returns>
        /// The person face entity.
        /// </returns>
        public async Task<PersonFace> GetPersonFaceAsync(string personGroupId, Guid personId, Guid faceId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}?{7}={8}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, FacesQuery, faceId, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, PersonFace>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Updates a face of a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="faceId">The face id.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task UpdatePersonFaceAsync(string personGroupId, Guid personId, Guid faceId, string userData)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}?{7}={8}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, FacesQuery, faceId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(new HttpMethod("PATCH"), requestUrl, new { userData = userData });
        }

        /// <summary>
        /// Deletes a face of a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="faceId">The face id.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task DeletePersonFaceAsync(string personGroupId, Guid personId, Guid faceId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}?{7}={8}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, FacesQuery, faceId, SubscriptionKeyName, this._subscriptionKey);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Finds the similar faces.
        /// </summary>
        /// <param name="faceId">The face identifier.</param>
        /// <param name="faceIds">The face ids.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task<SimilarFace[]> FindSimilarAsync(Guid faceId, Guid[] faceIds)
        {
            var requestUrl = string.Format("{0}/findsimilars?{1}={2}", ServiceHost, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, SimilarFace[]>(HttpMethod.Post, requestUrl, new { faceId = faceId, faceIds = faceIds });
        }

        /// <summary>
        /// Groups the face.
        /// </summary>
        /// <param name="faceIds">The face ids.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task<GroupResult> GroupAsync(Guid[] faceIds)
        {
            var requestUrl = string.Format("{0}/groupings?{1}={2}", ServiceHost, SubscriptionKeyName, this._subscriptionKey);

            return await this.SendRequestAsync<object, GroupResult>(HttpMethod.Post, requestUrl, new { faceIds = faceIds });
        }
        #endregion

        #region the json client
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, ServiceHost);
            request.RequestUri = new Uri(requestUrl);
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

            HttpResponseMessage response = await s_httpClient.SendAsync(request);
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
