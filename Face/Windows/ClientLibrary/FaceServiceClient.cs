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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class FaceServiceClient : IDisposable, IFaceServiceClient
    {
        #region Fields

        /// <summary>
        /// The default service host.
        /// </summary>
        private const string SERVICE_HOST = "https://api.projectoxford.ai/face/v1.0";

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
        /// The detect.
        /// </summary>
        private const string DetectQuery = "detect";

        /// <summary>
        /// The verify.
        /// </summary>
        private const string VerifyQuery = "verify";

        /// <summary>
        /// The train query.
        /// </summary>
        private const string TrainQuery = "train";

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
        /// The persisted faces query string.
        /// </summary>
        private const string PersistedFacesQuery = "persistedfaces";

        /// <summary>
        /// The face list query
        /// </summary>
        private const string FaceListsQuery = "facelists";

        /// <summary>
        /// The endpoint for Find Similar API.
        /// </summary>
        private const string FindSimilarsQuery = "findsimilars";

        /// <summary>
        /// The identify.
        /// </summary>
        private const string IdentifyQuery = "identify";

        /// <summary>
        /// The group.
        /// </summary>
        private const string GroupQuery = "group";

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
        /// The subscription key.
        /// </summary>
        private string _subscriptionKey;

        /// <summary>
        /// The HTTP client
        /// </summary>
        private HttpClient _httpClient;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public FaceServiceClient(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(SubscriptionKeyName, subscriptionKey);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FaceServiceClient"/> class.
        /// </summary>
        ~FaceServiceClient()
        {
            Dispose(false);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets service endpoint address, overridable by subclasses, default to free subscription's endpoint.
        /// </summary>
        protected virtual string ServiceHost => SERVICE_HOST;

        /// <summary>
        /// Gets default request headers for all following http request
        /// </summary>
        public HttpRequestHeaders DefaultRequestHeaders
        {
            get
            {
                return _httpClient.DefaultRequestHeaders;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets face attribute query string from attribute types
        /// </summary>
        /// <param name="types">Face attribute types</param>
        /// <returns>Face attribute query string</returns>
        public static string GetAttributeString(IEnumerable<FaceAttributeType> types)
        {
            return string.Join(",", types.Select(attr =>
                {
                    var attrStr = attr.ToString();
                    return char.ToLowerInvariant(attrStr[0]) + attrStr.Substring(1);
                }).ToArray());
        }

        /// <summary>
        /// Detects an URL asynchronously.
        /// </summary>
        /// <param name="imageUrl">The image URL.</param>
        /// <param name="returnFaceId">If set to <c>true</c> [return face ID].</param>
        /// <param name="returnFaceLandmarks">If set to <c>true</c> [return face landmarks].</param>
        /// <param name="returnFaceAttributes">Face attributes need to be returned.</param> 
        /// <returns>The detected faces.</returns>
        public async Task<Microsoft.ProjectOxford.Face.Contract.Face[]> DetectAsync(string imageUrl, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            if (returnFaceAttributes != null)
            {
                var requestUrl = string.Format(
                    "{0}/{1}?returnFaceId={2}&returnFaceLandmarks={3}&returnFaceAttributes={4}",
                    ServiceHost,
                    DetectQuery,
                    returnFaceId,
                    returnFaceLandmarks,
                    GetAttributeString(returnFaceAttributes));

                return await this.SendRequestAsync<object, Microsoft.ProjectOxford.Face.Contract.Face[]>(HttpMethod.Post, requestUrl, new { url = imageUrl });
            }
            else
            {
                var requestUrl = string.Format(
                    "{0}/{1}?returnFaceId={2}&returnFaceLandmarks={3}",
                    ServiceHost,
                    DetectQuery,
                    returnFaceId,
                    returnFaceLandmarks);

                return await this.SendRequestAsync<object, Microsoft.ProjectOxford.Face.Contract.Face[]>(HttpMethod.Post, requestUrl, new { url = imageUrl });
            }
        }

        /// <summary>
        /// Detects an image asynchronously.
        /// </summary>
        /// <param name="imageStream">The image stream.</param>
        /// <param name="returnFaceId">If set to <c>true</c> [return face ID].</param>
        /// <param name="returnFaceLandmarks">If set to <c>true</c> [return face landmarks].</param>
        /// <param name="returnFaceAttributes">Face attributes need to be returned.</param> 
        /// <returns>The detected faces.</returns>
        public async Task<Microsoft.ProjectOxford.Face.Contract.Face[]> DetectAsync(Stream imageStream, bool returnFaceId = true, bool returnFaceLandmarks = false, IEnumerable<FaceAttributeType> returnFaceAttributes = null)
        {
            if (returnFaceAttributes != null)
            {
                var requestUrl = string.Format(
                    "{0}/{1}?returnFaceId={2}&returnFaceLandmarks={3}&returnFaceAttributes={4}",
                    ServiceHost,
                    DetectQuery,
                    returnFaceId,
                    returnFaceLandmarks,
                    GetAttributeString(returnFaceAttributes));

                return await this.SendRequestAsync<Stream, Microsoft.ProjectOxford.Face.Contract.Face[]>(HttpMethod.Post, requestUrl, imageStream);
            }
            else
            {
                var requestUrl = string.Format(
                    "{0}/{1}?returnFaceId={2}&returnFaceLandmarks={3}",
                    ServiceHost,
                    DetectQuery,
                    returnFaceId,
                    returnFaceLandmarks);

                return await this.SendRequestAsync<Stream, Microsoft.ProjectOxford.Face.Contract.Face[]>(HttpMethod.Post, requestUrl, imageStream);
            }
        }

        /// <summary>
        /// Verifies whether the specified two faces belong to the same person asynchronously.
        /// </summary>
        /// <param name="faceId1">The face id 1.</param>
        /// <param name="faceId2">The face id 2.</param>
        /// <returns>The verification result.</returns>
        public async Task<VerifyResult> VerifyAsync(Guid faceId1, Guid faceId2)
        {
            var requestUrl = string.Format("{0}/{1}", ServiceHost, VerifyQuery);

            return await this.SendRequestAsync<object, VerifyResult>(
                HttpMethod.Post,
                requestUrl,
                new
                {
                    faceId1 = faceId1,
                    faceId2 = faceId2
                });
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
            var requestUrl = string.Format("{0}/{1}", ServiceHost, IdentifyQuery);

            return await this.SendRequestAsync<object, IdentifyResult[]>(
                HttpMethod.Post,
                requestUrl,
                new
                {
                    personGroupId = personGroupId,
                    faceIds = faceIds,
                    maxNumOfCandidatesReturned = maxNumOfCandidatesReturned
                });
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
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, PersonGroupsQuery, personGroupId);

            await this.SendRequestAsync<object, object>(
                HttpMethod.Put,
                requestUrl,
                new
                {
                    name = name,
                    userData = userData
                });
        }

        /// <summary>
        /// Gets a person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>The person group entity.</returns>
        public async Task<PersonGroup> GetPersonGroupAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, PersonGroupsQuery, personGroupId);

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
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, PersonGroupsQuery, personGroupId);

            await this.SendRequestAsync<object, object>(
                new HttpMethod("PATCH"),
                requestUrl,
                new
                {
                    name = name,
                    userData = userData
                });
        }

        /// <summary>
        /// Deletes a person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>Task object.</returns>
        public async Task DeletePersonGroupAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, PersonGroupsQuery, personGroupId);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Gets all person groups asynchronously.
        /// </summary>
        /// <returns>Person group entity array.</returns>
        public async Task<PersonGroup[]> GetPersonGroupsAsync()
        {
            var requestUrl = string.Format(
                "{0}/{1}",
                ServiceHost,
                PersonGroupsQuery);

            return await this.SendRequestAsync<object, PersonGroup[]>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Trains the person group asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>Task object.</returns>
        public async Task TrainPersonGroupAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}", ServiceHost, PersonGroupsQuery, personGroupId, TrainQuery);

            await this.SendRequestAsync<object, object>(HttpMethod.Post, requestUrl, null);
        }

        /// <summary>
        /// Gets person group training status asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <returns>The person group training status.</returns>
        public async Task<TrainingStatus> GetPersonGroupTrainingStatusAsync(string personGroupId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}", ServiceHost, PersonGroupsQuery, personGroupId, TrainingQuery);

            return await this.SendRequestAsync<object, TrainingStatus>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Creates a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>       
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>The CreatePersonResult entity.</returns>
        public async Task<CreatePersonResult> CreatePersonAsync(string personGroupId, string name, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery);

            return await this.SendRequestAsync<object, CreatePersonResult>(
                HttpMethod.Post,
                requestUrl,
                new
                {
                    name = name,
                    userData = userData
                });
        }

        /// <summary>
        /// Gets a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <returns>The person entity.</returns>
        public async Task<Person> GetPersonAsync(string personGroupId, Guid personId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId);

            return await this.SendRequestAsync<object, Person>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Updates a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>    
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>Task object.</returns>
        public async Task UpdatePersonAsync(string personGroupId, Guid personId, string name, string userData = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId);

            await this.SendRequestAsync<object, object>(
                new HttpMethod("PATCH"),
                requestUrl,
                new
                {
                    name = name,
                    userData = userData
                });
        }

        /// <summary>
        /// Deletes a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <returns>Task object.</returns>
        public async Task DeletePersonAsync(string personGroupId, Guid personId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId);

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
                "{0}/{1}/{2}/{3}",
                ServiceHost,
                PersonGroupsQuery,
                personGroupId,
                PersonsQuery);

            return await this.SendRequestAsync<object, Person[]>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Adds a face to a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="imageUrl">The face image URL.</param>    
        /// <param name="userData">The user data.</param>
        /// <param name="targetFace">The target face.</param>
        /// <returns>
        /// Add person face result.
        /// </returns>
        public async Task<AddPersistedFaceResult> AddPersonFaceAsync(string personGroupId, Guid personId, string imageUrl, string userData = null, FaceRectangle targetFace = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}?userData={6}&targetFace={7}", 
                ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, PersistedFacesQuery, userData, 
                targetFace == null ? string.Empty : string.Format("{0},{1},{2},{3}", targetFace.Left, targetFace.Top, targetFace.Width, targetFace.Height));

            return await this.SendRequestAsync<object, AddPersistedFaceResult>(HttpMethod.Post, requestUrl, new { url = imageUrl });
        }

        /// <summary>
        /// Adds a face to a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="imageStream">The face image stream.</param>    
        /// <param name="userData">The user data.</param>   
        /// <param name="targetFace">The target face.</param>
        /// <returns>
        /// Add person face result.
        /// </returns>
        public async Task<AddPersistedFaceResult> AddPersonFaceAsync(string personGroupId, Guid personId, Stream imageStream, string userData = null, FaceRectangle targetFace = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}?userData={6}&targetFace={7}", 
                ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, PersistedFacesQuery, userData, 
                targetFace == null ? string.Empty : string.Format("{0},{1},{2},{3}", targetFace.Left, targetFace.Top, targetFace.Width, targetFace.Height));

            return await this.SendRequestAsync<Stream, AddPersistedFaceResult>(HttpMethod.Post, requestUrl, imageStream);
        }

        /// <summary>
        /// Gets a face of a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="persistedFace">The persisted face id.</param>
        /// <returns>
        /// The person face entity.
        /// </returns>
        public async Task<PersonFace> GetPersonFaceAsync(string personGroupId, Guid personId, Guid persistedFace)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, PersistedFacesQuery, persistedFace);

            return await this.SendRequestAsync<object, PersonFace>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Updates a face of a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="persistedFaceId">The persisted face id.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task UpdatePersonFaceAsync(string personGroupId, Guid personId, Guid persistedFaceId, string userData)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, PersistedFacesQuery, persistedFaceId);

            await this.SendRequestAsync<object, object>(new HttpMethod("PATCH"), requestUrl, new { userData = userData });
        }

        /// <summary>
        /// Deletes a face of a person asynchronously.
        /// </summary>
        /// <param name="personGroupId">The person group id.</param>
        /// <param name="personId">The person id.</param>
        /// <param name="persistedFaceId">The persisted face id.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task DeletePersonFaceAsync(string personGroupId, Guid personId, Guid persistedFaceId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/{6}", ServiceHost, PersonGroupsQuery, personGroupId, PersonsQuery, personId, PersistedFacesQuery, persistedFaceId);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Finds the similar faces asynchronously.
        /// </summary>
        /// <param name="faceId">The face identifier.</param>
        /// <param name="faceIds">The face identifiers.</param>
        /// <param name="maxNumOfCandidatesReturned">The max number of candidates returned.</param>
        /// <returns>
        /// The similar faces.
        /// </returns>
        public async Task<SimilarFace[]> FindSimilarAsync(Guid faceId, Guid[] faceIds, int maxNumOfCandidatesReturned = 20)
        {
            var requestUrl = string.Format("{0}/{1}", ServiceHost, FindSimilarsQuery);

            return await this.SendRequestAsync<object, SimilarFace[]>(
                HttpMethod.Post,
                requestUrl,
                new
                {
                    faceId = faceId,
                    faceIds = faceIds,
                    maxNumOfCandidatesReturned = maxNumOfCandidatesReturned
                });
        }

        /// <summary>
        /// Finds the similar faces asynchronously.
        /// </summary>
        /// <param name="faceId">The face identifier.</param>
        /// <param name="faceListId">The face list identifier.</param>
        /// <param name="maxNumOfCandidatesReturned">The max number of candidates returned.</param>
        /// <returns>
        /// The similar persisted faces.
        /// </returns>
        public async Task<SimilarPersistedFace[]> FindSimilarAsync(Guid faceId, string faceListId, int maxNumOfCandidatesReturned = 20)
        {
            var requestUrl = string.Format("{0}/{1}", ServiceHost, FindSimilarsQuery);

            return await this.SendRequestAsync<object, SimilarPersistedFace[]>(
                HttpMethod.Post,
                requestUrl,
                new
                {
                    faceId = faceId,
                    faceListId = faceListId,
                    maxNumOfCandidatesReturned = maxNumOfCandidatesReturned
                });
        }

        /// <summary>
        /// Groups the face asynchronously.
        /// </summary>
        /// <param name="faceIds">The face ids.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task<GroupResult> GroupAsync(Guid[] faceIds)
        {
            var requestUrl = string.Format("{0}/{1}", ServiceHost, GroupQuery);

            return await this.SendRequestAsync<object, GroupResult>(
                HttpMethod.Post,
                requestUrl,
                new
                {
                    faceIds = faceIds
                });
        }

        /// <summary>
        /// Creates the face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>   
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task CreateFaceListAsync(string faceListId, string name, string userData)
        {
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, FaceListsQuery, faceListId);

            await this.SendRequestAsync<object, object>(
                HttpMethod.Put,
                requestUrl,
                new
                {
                    name = name,
                    userData = userData
                });
        }

        /// <summary>
        /// Gets the face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <returns>
        /// Face list object.
        /// </returns>
        public async Task<FaceList> GetFaceListAsync(string faceListId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, FaceListsQuery, faceListId);

            return await this.SendRequestAsync<object, FaceList>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// List the face lists asynchronously.
        /// </summary>
        /// <returns>
        /// FaceListMetadata array.
        /// </returns>
        public async Task<FaceListMetadata[]> ListFaceListsAsync()
        {
            var requestUrl = string.Format("{0}/{1}", ServiceHost, FaceListsQuery);

            return await this.SendRequestAsync<object, FaceListMetadata[]>(HttpMethod.Get, requestUrl, null);
        }

        /// <summary>
        /// Updates the face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="userData">The user data.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task UpdateFaceListAsync(string faceListId, string name, string userData)
        {
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, FaceListsQuery, faceListId);

            await this.SendRequestAsync<object, object>(
                new HttpMethod("PATCH"),
                requestUrl,
                new
                {
                    name = name,
                    userData = userData
                });
        }

        /// <summary>
        /// Deletes the face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <returns>
        /// Task object.
        /// </returns>
        public async Task DeleteFaceListAsync(string faceListId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}", ServiceHost, FaceListsQuery, faceListId);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Adds the face to face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <param name="imageUrl">The face image URL.</param>    
        /// <param name="userData">The user data.</param>
        /// <param name="targetFace">The target face.</param>     
        /// <returns>
        /// Add face result.
        /// </returns>
        public async Task<AddPersistedFaceResult> AddFaceToFaceListAsync(string faceListId, string imageUrl, string userData = null, FaceRectangle targetFace = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}?userData={4}&targetFace={5}", 
                ServiceHost, FaceListsQuery, faceListId, PersistedFacesQuery, userData,
                targetFace == null ? string.Empty : string.Format("{0},{1},{2},{3}", targetFace.Left, targetFace.Top, targetFace.Width, targetFace.Height));

            return await this.SendRequestAsync<object, AddPersistedFaceResult>(HttpMethod.Post, requestUrl, new { url = imageUrl });
        }

        /// <summary>
        /// Adds the face to face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <param name="imageStream">The face image stream.</param>   
        /// <param name="userData">The user data.</param>   
        /// <param name="targetFace">The target face.</param>     
        /// <returns>
        /// Add face result.
        /// </returns>
        public async Task<AddPersistedFaceResult> AddFaceToFaceListAsync(string faceListId, Stream imageStream, string userData = null, FaceRectangle targetFace = null)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}?userData={4}&targetFace={5}", 
                ServiceHost, FaceListsQuery, faceListId, PersistedFacesQuery, userData, 
                targetFace == null ? string.Empty : string.Format("{0},{1},{2},{3}", targetFace.Left, targetFace.Top, targetFace.Width, targetFace.Height));

            return await this.SendRequestAsync<object, AddPersistedFaceResult>(HttpMethod.Post, requestUrl, imageStream);
        }

        /// <summary>
        /// Deletes the face from face list asynchronously.
        /// </summary>
        /// <param name="faceListId">The face list identifier.</param>
        /// <param name="persistedFaceId">The persisted face id.</param>
        /// <returns>Task object.</returns>
        public async Task DeleteFaceFromFaceListAsync(string faceListId, Guid persistedFaceId)
        {
            var requestUrl = string.Format("{0}/{1}/{2}/{3}/{4}", ServiceHost, FaceListsQuery, faceListId, PersistedFacesQuery, persistedFaceId);

            await this.SendRequestAsync<object, object>(HttpMethod.Delete, requestUrl, null);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                }
            }
        }

        /// <summary>
        /// Sends the request asynchronous.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TResponse">The type of the response.</typeparam>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <param name="requestBody">The request body.</param>
        /// <returns>The response.</returns>
        /// <exception cref="OxfordAPIException">The client exception.</exception>
        private async Task<TResponse> SendRequestAsync<TRequest, TResponse>(HttpMethod httpMethod, string requestUrl, TRequest requestBody)
        {
            var request = new HttpRequestMessage(httpMethod, ServiceHost);
            request.RequestUri = new Uri(requestUrl);
            if (requestBody != null)
            {
                if (requestBody is Stream)
                {
                    request.Content = new StreamContent(requestBody as Stream);
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue(StreamContentTypeHeader);
                }
                else
                {
                    request.Content = new StringContent(JsonConvert.SerializeObject(requestBody, s_settings), Encoding.UTF8, JsonContentTypeHeader);
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
                if (response.Content != null && response.Content.Headers.ContentType.MediaType.Contains(JsonContentTypeHeader))
                {
                    var errorObjectString = await response.Content.ReadAsStringAsync();
                    ClientError ex = JsonConvert.DeserializeObject<ClientError>(errorObjectString);
                    if (ex.Error != null)
                    {
                        throw new FaceAPIException(ex.Error.ErrorCode, ex.Error.Message, response.StatusCode);
                    }
                    else
                    {
                        ServiceError serviceEx = JsonConvert.DeserializeObject<ServiceError>(errorObjectString);
                        if (ex != null)
                        {
                            throw new FaceAPIException(serviceEx.ErrorCode, serviceEx.Message, response.StatusCode);
                        }
                        else
                        {
                            throw new FaceAPIException("Unknown", "Unknown Error", response.StatusCode);
                        }
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            return default(TResponse);
        }

        #endregion Methods
    }
}