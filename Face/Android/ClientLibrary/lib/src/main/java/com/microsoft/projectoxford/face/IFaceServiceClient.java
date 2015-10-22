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

import com.microsoft.projectoxford.face.contract.CreatePersonResult;
import com.microsoft.projectoxford.face.contract.Face;
import com.microsoft.projectoxford.face.contract.GroupResult;
import com.microsoft.projectoxford.face.contract.IdentifyResult;
import com.microsoft.projectoxford.face.contract.Person;
import com.microsoft.projectoxford.face.contract.PersonFace;
import com.microsoft.projectoxford.face.contract.PersonGroup;
import com.microsoft.projectoxford.face.contract.SimilarFace;
import com.microsoft.projectoxford.face.contract.TrainingStatus;
import com.microsoft.projectoxford.face.contract.VerifyResult;
import com.microsoft.projectoxford.face.rest.RESTException;

import java.io.IOException;
import java.io.InputStream;
import java.util.UUID;

public interface IFaceServiceClient {

    public Face[] detect(String url, boolean analyzesFacialLandmarks, boolean analyzesAge, boolean analyzesGender, boolean analyzesHeadPose) throws RESTException;

    public Face[] detect(InputStream image, boolean analyzesFacialLandmarks, boolean analyzesAge, boolean analyzesGender, boolean analyzesHeadPose) throws RESTException, IOException;

    public VerifyResult verify(UUID faceId1, UUID faceId2) throws RESTException;

    public IdentifyResult[] identity(String personGroupId, UUID[] faceIds, int maxNumOfCandidatesReturned) throws RESTException;

    public TrainingStatus trainPersonGroup(String personGroupId) throws RESTException;

    public TrainingStatus getPersonGroupTrainingStatus(String personGroupId) throws RESTException;

    public void createPersonGroup(String personGroupId, String name, String userData) throws RESTException;

    public void deletePersonGroup(String personGroupId) throws RESTException;

    public void updatePersonGroup(String personGroupId, String name, String userData) throws RESTException;

    public PersonGroup getPersonGroup(String personGroupId) throws RESTException;

    public PersonGroup[] getPersonGroups() throws RESTException;

    public CreatePersonResult createPerson(String personGroupId, UUID[] faceIds, String name, String userData) throws RESTException;

    public Person getPerson(String personGroupId, UUID personId) throws RESTException;

    public Person[] getPersons(String personGroupId) throws RESTException;

    public void addPersonFace(String personGroupId, UUID personId, UUID faceId, String userData) throws RESTException;

    public PersonFace getPersonFace(String personGroupId, UUID personId, UUID faceId) throws RESTException;

    public void updatePersonFace(String personGroupId, UUID personId, UUID faceId, String userData) throws RESTException;

    public void updatePerson(String personGroupId, UUID personId, UUID[] faceIds, String name, String userData) throws RESTException;

    public void deletePerson(String personGroupId, UUID personId) throws RESTException;

    public void deletePersonFace(String personGroupId, UUID personId, UUID faceId) throws RESTException;

    public SimilarFace[] findSimilar(UUID faceId, UUID[] faceIds) throws RESTException;

    public GroupResult group(UUID[] faceIds) throws RESTException;
}
