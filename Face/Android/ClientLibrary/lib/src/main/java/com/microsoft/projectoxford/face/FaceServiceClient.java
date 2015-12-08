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
package com.microsoft.projectoxford.face;

import com.microsoft.projectoxford.face.contract.AddPersistedFaceResult;
import com.microsoft.projectoxford.face.contract.CreatePersonResult;
import com.microsoft.projectoxford.face.contract.Face;
import com.microsoft.projectoxford.face.contract.FaceList;
import com.microsoft.projectoxford.face.contract.FaceListMetadata;
import com.microsoft.projectoxford.face.contract.FaceRectangle;
import com.microsoft.projectoxford.face.contract.GroupResult;
import com.microsoft.projectoxford.face.contract.IdentifyResult;
import com.microsoft.projectoxford.face.contract.Person;
import com.microsoft.projectoxford.face.contract.PersonFace;
import com.microsoft.projectoxford.face.contract.PersonGroup;
import com.microsoft.projectoxford.face.contract.SimilarFace;
import com.microsoft.projectoxford.face.contract.SimilarPersistedFace;
import com.microsoft.projectoxford.face.contract.TrainingStatus;
import com.microsoft.projectoxford.face.contract.VerifyResult;
import com.microsoft.projectoxford.face.rest.ClientException;

import java.io.IOException;
import java.io.InputStream;
import java.util.Collection;
import java.util.UUID;

public interface FaceServiceClient {

    /**
     * Supported face attribute types
     */
    public enum FaceAttributeType
    {
        /**
         * Analyses age
         */
        Age {
            public String toString() {
                return "age";
            }
        },

        /**
         * Analyses gender
         */
        Gender {
            public String toString() {
                return "gender";
            }
        },

        /**
         * Analyses facial hair
         */
        FacialHair {
            public String toString() {
                return "facialHair";
            }
        },

        /**
         * Analyses whether is smiling
         */
        Smile {
            public String toString() {
                return "smile";
            }
        },

        /**
         * Analyses head pose
         */
        HeadPose {
            public String toString() {
                return "headPose";
            }
        }
    }

    /**
     * Detects faces in an URL image.
     * @param url url.
     * @param returnFaceId If set to <c>true</c> [return face ID].
     * @param returnFaceLandmarks If set to <c>true</c> [return face landmarks].
     * @param returnFaceAttributes Return face attributes.
     * @return detected faces.
     * @throws ClientException
     * @throws IOException
     */
    Face[] detect(String url, boolean returnFaceId, boolean returnFaceLandmarks, FaceAttributeType[] returnFaceAttributes) throws ClientException, IOException;

    /**
     * Detects faces in an uploaded image.
     * @param imageStream The image stream.
     * @param returnFaceId If set to <c>true</c> [return face ID].
     * @param returnFaceLandmarks If set to <c>true</c> [return face landmarks]
     * @param returnFaceAttributes Return face attributes.
     * @return detected faces.
     * @throws ClientException
     * @throws IOException
     */
    Face[] detect(InputStream imageStream, boolean returnFaceId, boolean returnFaceLandmarks, FaceAttributeType[] returnFaceAttributes) throws ClientException, IOException;

    /**
     * Verifies whether the specified two faces belong to the same person.
     * @param faceId1 The face id 1.
     * @param faceId2 The face id 2.
     * @return The verification result.
     * @throws ClientException
     * @throws IOException
     */
    VerifyResult verify(UUID faceId1, UUID faceId2) throws ClientException, IOException;

    /**
     * Identities the faces in a given person group.
     * @param personGroupId The person group id.
     * @param faceIds The face ids.
     * @param maxNumOfCandidatesReturned The maximum number of candidates returned for each face.
     * @return The identification results.
     * @throws ClientException
     * @throws IOException
     */
    IdentifyResult[] identity(String personGroupId, UUID[] faceIds, int maxNumOfCandidatesReturned) throws ClientException, IOException;

    /**
     * Trains the person group.
     * @param personGroupId The person group id
     * @throws ClientException
     * @throws IOException
     */
    void trainPersonGroup(String personGroupId) throws ClientException, IOException;

    /**
     * Gets person group training status.
     * @param personGroupId The person group id.
     * @return The person group training status.
     * @throws ClientException
     * @throws IOException
     */
    TrainingStatus getPersonGroupTrainingStatus(String personGroupId) throws ClientException, IOException;

    /**
     *  Creates the person group.
     * @param personGroupId The person group identifier.
     * @param name The name.
     * @param userData The user data.
     * @throws ClientException
     * @throws IOException
     */
    void createPersonGroup(String personGroupId, String name, String userData) throws ClientException, IOException;

    /**
     * Deletes a person group.
     * @param personGroupId The person group id.
     * @throws ClientException
     * @throws IOException
     */
    void deletePersonGroup(String personGroupId) throws ClientException, IOException;

    /**
     * Updates a person group.
     * @param personGroupId The person group id.
     * @param name The name.
     * @param userData The user data.
     * @throws ClientException
     * @throws IOException
     */
    void updatePersonGroup(String personGroupId, String name, String userData) throws ClientException, IOException;

    /**
     * Gets a person group.
     * @param personGroupId The person group id.
     * @return The person group entity.
     * @throws ClientException
     * @throws IOException
     */
    PersonGroup getPersonGroup(String personGroupId) throws ClientException, IOException;

    /**
     *  Gets all person groups.
     * @return Person group entity array.
     * @throws ClientException
     * @throws IOException
     */
    PersonGroup[] getPersonGroups() throws ClientException, IOException;

    /**
     * Creates a person.
     * @param personGroupId The person group id.
     * @param name The name.
     * @param userData The user data.
     * @return The CreatePersonResult entity.
     * @throws ClientException
     * @throws IOException
     */
    CreatePersonResult createPerson(String personGroupId, String name, String userData) throws ClientException, IOException;

    /**
     *  Gets a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @return The person entity.
     * @throws ClientException
     * @throws IOException
     */
    Person getPerson(String personGroupId, UUID personId) throws ClientException, IOException;

    /**
     * Gets all persons inside a person group.
     * @param personGroupId The person group id.
     * @return The person entity array.
     * @throws ClientException
     * @throws IOException
     */
    Person[] getPersons(String personGroupId) throws ClientException, IOException;

    /**
     * Adds a face to a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @param url The face image URL.
     * @param userData The user data.
     * @param targetFace The target face.
     * @return Add person face result.
     * @throws ClientException
     * @throws IOException
     */
    AddPersistedFaceResult addPersonFace(String personGroupId, UUID personId, String url, String userData, FaceRectangle targetFace) throws ClientException, IOException;

    /**
     * Adds a face to a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @param imageStream The face image stream
     * @param userData The user data.
     * @param targetFace The Target Face.
     * @return Add person face result.
     * @throws ClientException
     * @throws IOException
     */
    AddPersistedFaceResult addPersonFace(String personGroupId, UUID personId, InputStream imageStream, String userData, FaceRectangle targetFace) throws ClientException, IOException;

    /**
     * Gets a face of a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @param persistedFaceId The persisted face id.
     * @return The person face entity.
     * @throws ClientException
     * @throws IOException
     */
    PersonFace getPersonFace(String personGroupId, UUID personId, UUID persistedFaceId) throws ClientException, IOException;

    /**
     * Updates a face of a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @param persistedFaceId The persisted face id.
     * @param userData The person face entity.
     * @throws ClientException
     * @throws IOException
     */
    void updatePersonFace(String personGroupId, UUID personId, UUID persistedFaceId, String userData) throws ClientException, IOException;

    /**
     * Updates a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @param name The name.
     * @param userData The user data.
     * @throws ClientException
     * @throws IOException
     */
    void updatePerson(String personGroupId, UUID personId, String name, String userData) throws ClientException, IOException;

    /**
     * Deletes a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @throws ClientException
     * @throws IOException
     */
    void deletePerson(String personGroupId, UUID personId) throws ClientException, IOException;

    /**
     * Deletes a face of a person.
     * @param personGroupId The person group id.
     * @param personId The person id.
     * @param persistedFaceId The persisted face id.
     * @throws ClientException
     * @throws IOException
     */
    void deletePersonFace(String personGroupId, UUID personId, UUID persistedFaceId) throws ClientException, IOException;

    /**
     * Finds the similar faces.
     * @param faceId The face identifier.
     * @param faceIds The face list identifier.
     * @param maxNumOfCandidatesReturned The max number of candidates returned.
     * @return The similar persisted faces.
     * @throws ClientException
     * @throws IOException
     */
    SimilarFace[] findSimilar(UUID faceId, UUID[] faceIds, int maxNumOfCandidatesReturned) throws ClientException, IOException;

    /**
     * Finds the similar faces.
     * @param faceId The face identifier.
     * @param faceListId The face list identifier.
     * @param maxNumOfCandidatesReturned The max number of candidates returned.
     * @return The similar persisted faces.
     * @throws ClientException
     * @throws IOException
     */
    SimilarPersistedFace[] findSimilar(UUID faceId, String faceListId, int maxNumOfCandidatesReturned) throws ClientException, IOException;

    /**
     * Groups the face.
     * @param faceIds The face ids.
     * @return Group result.
     * @throws ClientException
     * @throws IOException
     */
    GroupResult group(UUID[] faceIds) throws ClientException, IOException;

    /**
     *  Creates the face list.
     * @param faceListId The face list identifier.
     * @param name The name.
     * @param userData The user data.
     * @throws ClientException
     * @throws IOException
     */
    void createFaceList(String faceListId, String name, String userData) throws ClientException, IOException;

    /**
     * Gets the face list.
     * @param faceListId The face list identifier.
     * @return Face list object.
     * @throws ClientException
     * @throws IOException
     */
    FaceList getFaceList(String faceListId) throws ClientException, IOException;

    /**
     * Lists the face lists.
     * @return Face list metadata objects.
     * @throws ClientException
     * @throws IOException
     */
    FaceListMetadata[] listFaceLists() throws ClientException, IOException;

    /**
     * Updates the face list.
     * @param faceListId The face list identifier.
     * @param name The name.
     * @param userData The user data.
     * @throws ClientException
     * @throws IOException
     */
    void updateFaceList(String faceListId, String name, String userData) throws ClientException, IOException;

    /**
     * Deletes the face list
     * @param faceListId The face group identifier.
     * @throws ClientException
     * @throws IOException
     */
    void deleteFaceList(String faceListId) throws ClientException, IOException;

    /**
     * Adds the face to face list.
     * @param faceListId he face list identifier.
     * @param url The face image URL.
     * @param userData The user data.
     * @param targetFace
     * @return The target face.
     * @throws ClientException
     * @throws IOException
     */
    AddPersistedFaceResult addFacesToFaceList(String faceListId, String url, String userData, FaceRectangle targetFace) throws ClientException, IOException;

    /**
     *  Adds the face to face list
     * @param faceListId The face list identifier.
     * @param imageStream The face image stream.
     * @param userData The user data.
     * @param targetFace The target face.
     * @return Add face result.
     * @throws ClientException
     * @throws IOException
     */
    AddPersistedFaceResult AddFaceToFaceList(String faceListId, InputStream imageStream, String userData, FaceRectangle targetFace) throws ClientException, IOException;

    /**
     * Deletes the face from face list.
     * @param faceListId The face list identifier.
     * @param persistedFaceId The face identifier
     * @throws ClientException
     * @throws IOException
     */
    void deleteFacesFromFaceList(String faceListId, UUID persistedFaceId) throws ClientException, IOException;
}
