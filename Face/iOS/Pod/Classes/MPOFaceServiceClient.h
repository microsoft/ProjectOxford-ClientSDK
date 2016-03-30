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

#import <Foundation/Foundation.h>
#import "MPOFaceSDK.h"
#import "MPOPersonGroup.h"
#import "MPOTrainingStatus.h"
#import "MPOCreatePersonResult.h"
#import "MPOPerson.h"
#import "MPOPersonFace.h"
#import "MPOAddPersistedFaceResult.h"
#import "MPOFaceList.h"
#import "MPOFaceListMetadata.h"
typedef enum
{
    MPOFaceAttributeTypeAge = 1,
    MPOFaceAttributeTypeGender,
    MPOFaceAttributeTypeFacialHair,
    MPOFaceAttributeTypeSmile,
    MPOFaceAttributeTypeHeadPose,
} MPOFaceAttributeType;

typedef void (^MPOCompletionBlock)(NSError *error);
typedef void (^MPOFaceArrayCompletionBlock)(NSArray<MPOFace *> *collection, NSError *error);
typedef void (^MPOSimilarFaceArrayCompletionBlock)(NSArray<MPOSimilarFace *> *collection, NSError *error);
typedef void (^MPOIdentifyResultArrayCompletionBlock)(NSArray<MPOIdentifyResult *> *collection, NSError *error);
typedef void (^MPOPersonGroupArrayCompletionBlock)(NSArray<MPOPersonGroup *> *collection, NSError *error);
typedef void (^MPOPersonArrayCompletionBlock)(NSArray<MPOPerson *> *collection, NSError *error);
typedef void (^MPOFaceListMetadataArrayCompletionBlock)(NSArray<MPOFaceListMetadata *> *collection, NSError *error);

@interface MPOFaceServiceClient : NSObject

/**
*  Creates an instance of MPOFaceServiceClient
*  @param key @param key subscription key to use face api
*  @return @return MPOFaceServiceClient
*/
- (id)initWithSubscriptionKey:(NSString *)key;


#pragma mark Face Detection APIs


/**
 *  Detects faces in an URL image
 *  @param url                  url
 *  @param returnFaceId         if set to true, return faceId
 *  @param returnFaceLandmarks  if set to true, return face landmarks
 *  @param returnFaceAttributes return face attributes
 *  @param completion           completion handler
 *  @return detected faces, NSArray containing MPOFace objects
 */
- (NSURLSessionDataTask *)detectWithUrl:(NSString *)url returnFaceId:(BOOL)returnFaceId returnFaceLandmarks:(BOOL)returnFaceLandmarks returnFaceAttributes:(NSArray *)returnFaceAttributes completionBlock:(MPOFaceArrayCompletionBlock)completion;

/**
 *  Detects faces in an data stream
 *  @param data                 data
 *  @param returnFaceId         if set to true, return faceId
 *  @param returnFaceLandmarks  if set to true, return face landmarks
 *  @param returnFaceAttributes return face attributes
 *  @param completion           completion handler
 *  @return detected faces, NSArray containing MPOFace objects
 */
- (NSURLSessionDataTask *)detectWithData:(NSData *)data returnFaceId:(BOOL)returnFaceId returnFaceLandmarks:(BOOL)returnFaceLandmarks returnFaceAttributes:(NSArray *)returnFaceAttributes completionBlock:(MPOFaceArrayCompletionBlock)completion;

/**
 *  Verifies whether the specified two faces belong to the same person.
 *  @param faceId1    first face Id
 *  @param faceId2    second face Id
 *  @param completion completionHandler
 *  @return the verification result, MPOVerifyResult
 */
- (NSURLSessionDataTask *)verifyWithFirstFaceId:(NSString *)faceId1 faceId2:(NSString *)faceId2 completionBlock:(void (^) (MPOVerifyResult *verifyResult, NSError *error))completion;

/**
 *  Finds the similar faces.
 *  @param faceId     The face identifier.
 *  @param faceIds    The face list identifier.
 *  @param completion completionHandler
 *  @return the similar faces, NSArray containg MPOSimilarFace
 */
- (NSURLSessionDataTask *)findSimilarWithFaceId:(NSString *)faceId faceIds:(NSArray *)faceIds completionBlock:(MPOSimilarFaceArrayCompletionBlock)completion;

/**
 *  Groups the faces.
 *  @param faceIds    the face ids.
 *  @param completion completionHandler
 *  @return group result, MPOGroupResult
 */
- (NSURLSessionDataTask *)groupWithFaceIds:(NSArray *)faceIds completionBlock:(void (^) (MPOGroupResult *groupResult, NSError *error))completion;

/**
 *  Identities the faces in a given person group.
 *  @param personGroupId         the person group id
 *  @param faceIds               the face ids
 *  @param maxNumberOfCandidates The maximum number of candidates returned for each face.
 *  @param completion            completionHandler
 *  @return the identification results, NSArray containing MPOIdentifyResult objects
 */
- (NSURLSessionDataTask *)identifyWithPersonGroupId:(NSString *)personGroupId faceIds:(NSArray *)faceIds maxNumberOfCandidates:(NSInteger)maxNumberOfCandidates completionBlock:(MPOIdentifyResultArrayCompletionBlock)completion;


#pragma mark Person Groups APIs


/**
 *  Creates the person group
 *  @param personGroupId the group identifier.
 *  @param name          the name of the person group
 *  @param userData      the user data
 *  @param completion    completionHandler
 *  @return boolean representing success or failure of creation
 */
- (NSURLSessionDataTask *)createPersonGroupWithId:(NSString *)personGroupId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Gets a person group
 *  @param personGroupId the person group id
 *  @param completion    completionHandler
 *  @return The person group, MPOPersonGroup
 */
- (NSURLSessionDataTask *)getPersonGroupWithPersonGroupId:(NSString *)personGroupId completionBlock:(void (^) (MPOPersonGroup *personGroup, NSError *error))completion;

/**
 *  Updates a person group.
 *  @param personGroupId the person group id
 *  @param name          the name
 *  @param userData      the user data
 *  @param completion    completionHandler
 *  @return boolean representing success or failure of update
 */
- (NSURLSessionDataTask *)updatePersonGroupWithPersonGroupId:(NSString *)personGroupId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Deletes a person group.
 *  @param personGroupId the person group id
 *  @param completion    completionHandler
 *  @return boolean representing success or failure of deletion
 */
- (NSURLSessionDataTask *)deletePersonGroupWithPersonGroupId:(NSString *)personGroupId completionBlock:(MPOCompletionBlock)completion;

/**
 *  Gets all person groups
 *  @param completion completionHandler
 *  @return person group entities, NSArray of MPOPersonGroup objects
 */
- (NSURLSessionDataTask *)getPersonGroupsWithCompletion:(MPOPersonGroupArrayCompletionBlock)completion;

/**
 *  Trains the person group
 *  @param personGroupId the person group id
 *  @param completion    completionHandler
 *  @return boolean representing success or failure of starting train operation
 */
- (NSURLSessionDataTask *)trainPersonGroupWithPersonGroupId:(NSString *)personGroupId completionBlock:(MPOCompletionBlock)completion;

/**
 *  Gets person group training status
 *  @param personGroupId the person group id
 *  @param completion    completionHandler
 *  @return the person group training status, MPOTrainingStatus
 */
- (NSURLSessionDataTask *)getPersonGroupTrainingStatusWithPersonGroupId:(NSString *)personGroupId completionBlock:(void (^) (MPOTrainingStatus *trainingStatus, NSError *error))completion;


#pragma mark Person APIs


/**
 *  Creates a person
 *  @param personGroupId the person group id
 *  @param name          the name
 *  @param userData      the user data
 *  @param completion    completionHandler
 *  @return the create person result, MPOCreatePersonResult
 */
- (NSURLSessionDataTask *)createPersonWithPersonGroupId:(NSString *)personGroupId name:(NSString *)name userData:(NSString *)userData completionBlock:(void (^) (MPOCreatePersonResult *createPersonResult, NSError *error))completion;

/**
 *  Gets a person
 *  @param personGroupId the person group id
 *  @param personId      the person id
 *  @param completion    completionHandler
 *  @return the person, MPOPerson
 */
- (NSURLSessionDataTask *)getPersonWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId completionBlock:(void (^) (MPOPerson *person, NSError *error))completion;

/**
 *  Updates person.
 *  @param personGroupId the person group id
 *  @param personId      the person id
 *  @param name          the name
 *  @param userData      the user data
 *  @param completion    completionHandler
 *  @return boolean representing success or failure of update person operation
 */
- (NSURLSessionDataTask *)updatePersonWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Deletes a person.
 *  @param personGroupId the person group id
 *  @param personId      the person id
 *  @param completion    completionHandler
 *  @return boolean representing success or failure of delete person operation
 */
- (NSURLSessionDataTask *)deletePersonWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId completionBlock:(MPOCompletionBlock)completion;

/**
 *  Gets all persons inside a person group
 *  @param personGroupId the person group id
 *  @param completion    completion handler
 *  @return the persons, NSArray containing MPOPerson objects
 */
- (NSURLSessionDataTask *)getPersonsWithPersonGroupId:(NSString *)personGroupId completionBlock:(MPOPersonArrayCompletionBlock)completion;

/**
 *  Gets a face of a person
 *  @param personGroupId   the person group id
 *  @param personId        the person id
 *  @param persistedFaceId the persisted face id
 *  @param completion       completionHandler
 *  @return the person face, MPOPersonFace
 */
- (NSURLSessionDataTask *)getPersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId persistedFaceId:(NSString *)persistedFaceId completionBlock:(void (^) (MPOPersonFace *personFace, NSError *error))completion;

/**
 *  Updates a face of a person
 *  @param personGroupId   the person group id
 *  @param personId        the person id
 *  @param persistedFaceId the persisted face id
 *  @param userData        the user data
 *  @param completion      completionHandler
 *  @return boolean representing success or failure of update person face operation
 */
- (NSURLSessionDataTask *)updatePersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId persistedFaceId:(NSString *)persistedFaceId userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Deletes a face of a person
 *  @param personGroupId   The person group id
 *  @param personId        The person id
 *  @param persistedFaceId The persisted face id
 *  @param completion      completionHandler
 *  @return boolean representing success or failure of delete person face operation
 */
- (NSURLSessionDataTask *)deletePersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId persistedFaceId:(NSString *)persistedFaceId completionBlock:(MPOCompletionBlock)completion;

/**
 *  Adds a face to a person.
 *  @param personGroupId The person group id
 *  @param personId      The person id
 *  @param url           The face image URL.
 *  @param userData      The user data.
 *  @param faceRectangle The face rectangle.
 *  @param completion    completionHandler
 *  @return Add Persisted Face Result, MPOAddPersistedFaceResult
 */
- (NSURLSessionDataTask *)addPersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId url:(NSString *)url userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion;

/**
 *  Adds a face to a person.
 *  @param personGroupId The person group id
 *  @param personId      The person id
 *  @param data          The face data.
 *  @param userData      The user data.
 *  @param faceRectangle The face rectangle.
 *  @param completion    completionHandler
 *  @return Add Persisted Face Result, MPOAddPersistedFaceResult
 */
- (NSURLSessionDataTask *)addPersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId data:(NSData *)data userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion;


#pragma mark Face List APIs


/**
 *  Creates the face list
 *  @param faceListId The face list identifier.
 *  @param name       The name
 *  @param userData   The user data.
 *  @param completion completionHandler
 *  @return boolean representing success or failure of create face list operation
 */
- (NSURLSessionDataTask *)createFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Gets the face list
 *  @param faceListId The face list identifier.
 *  @param completion completionHandler
 *  @return The face list, MPOFaceList
 */
- (NSURLSessionDataTask *)getFaceListWithFaceListId:(NSString *)faceListId completionBlock:(void (^) (MPOFaceList *addPersistedFaceResult, NSError *error))completion;

/**
 *  Lists all face lists
 *  @param completion completionHandler
 *  @return face lists, NSArray containing MPOFaceListMetadata objects
 */
- (NSURLSessionDataTask *)listFaceListsWithCompletion:(MPOFaceListMetadataArrayCompletionBlock)completion;

/**
 *  Updates a face list.
 *  @param faceListId The face list identifier.
 *  @param name       the name
 *  @param userData   the user data
 *  @param completion the completionHandler
 *  @return boolean representing success or failure of update face list operation
 */
- (NSURLSessionDataTask *)updateFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Deletes a face list.
 *  @param faceListId The face list identifier.
 *  @param name       the name
 *  @param userData   the user data.
 *  @param completion the completionHandler.
 *  @return boolean representing success or failure of delete face list operation
 */
- (NSURLSessionDataTask *)deleteFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion;

/**
 *  Deletes faces from a face list
 *  @param faceListId      the face list identifier.
 *  @param name            the name
 *  @param persistedFaceId the face identifier.
 *  @param completion      completionHandler
 *  @return boolean representing success or failure of delete face from face list operation
 */
- (NSURLSessionDataTask *)deleteFacesFromFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name persistedFaceId:(NSString *)persistedFaceId completionBlock:(MPOCompletionBlock)completion;

/**
 *  Adds the face to the face list.
 *  @param faceListId    the face list identifier.
 *  @param url           the face image URL.
 *  @param userData      the user data.
 *  @param faceRectangle the face rectangle.
 *  @param completion    completionHandler
 *  @return the persisted face result, MPOAddPersistedFaceResult
 */
- (NSURLSessionDataTask *)addFacesToFaceListWithFaceListId:(NSString *)faceListId url:(NSString *)url userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion;

/**
 *  Adds the face to the face list.
 *  @param faceListId    the face list identifier.
 *  @param data          the face image data.
 *  @param userData      the user data.
 *  @param faceRectangle the face rectangle.
 *  @param completion    completionHandler
 *  @return the persisted face result, MPOAddPersistedFaceResult
 */
- (NSURLSessionDataTask *)addFacesToFaceListWithFaceListId:(NSString *)faceListId data:(NSData *)data userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion;

@end