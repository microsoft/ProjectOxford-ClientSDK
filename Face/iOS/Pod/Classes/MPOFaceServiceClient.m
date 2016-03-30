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

#import "MPOFaceServiceClient.h"

@interface MPOFaceServiceClient ()
//private properties
typedef void(^PORequestCompletionBlock)(NSURLResponse *response, id responseObject, NSError *error);
@property NSString* subscriptionKey;
@end


@implementation MPOFaceServiceClient

- (id)initWithSubscriptionKey:(NSString *)key {
    
    self = [super init];
    if (self) {
        self.subscriptionKey = key;
    }
    
    return self;
}

#pragma mark Face
/*
 * =============================================================
 * ================ Face =================
 * =============================================================
 */

- (NSString *)faceAttributesWithArray:(NSArray *)faceAttributeTypesArray {
    NSMutableArray *faceAttributesStringArray = [[NSMutableArray alloc] init];
    for (NSNumber *number in faceAttributeTypesArray) {
        
        if ([number isEqualToNumber:@(MPOFaceAttributeTypeAge)]) {
            [faceAttributesStringArray addObject:@"age"];
        }
        
        if ([number isEqualToNumber:@(MPOFaceAttributeTypeGender)]) {
            [faceAttributesStringArray addObject:@"gender"];
        }
        if ([number isEqualToNumber:@(MPOFaceAttributeTypeHeadPose)]) {
            [faceAttributesStringArray addObject:@"headPose"];

        }
        if ([number isEqualToNumber:@(MPOFaceAttributeTypeSmile)]) {
            [faceAttributesStringArray addObject:@"smile"];

        }
        if ([number isEqualToNumber:@(MPOFaceAttributeTypeFacialHair)]) {
            [faceAttributesStringArray addObject:@"facialHair"];
        }
        
    }
    
    NSString *joinedComponents = [faceAttributesStringArray componentsJoinedByString:@","];
    
    return joinedComponents;
}

- (NSString *)faceRectangleStringRepresentation:(MPOFaceRectangle *)faceRectangle {
    if (!faceRectangle) {
        return nil;
    }
    else {
        return [NSString stringWithFormat:@"%@,%@,%@,%@", faceRectangle.left.stringValue, faceRectangle.top.stringValue, faceRectangle.width.stringValue, faceRectangle.height.stringValue];
    }
}

- (NSURLSessionDataTask *)detectWithUrl:(NSString *)url returnFaceId:(BOOL)returnFaceId returnFaceLandmarks:(BOOL)returnFaceLandmarks returnFaceAttributes:(NSArray *)returnFaceAttributes completionBlock:(MPOFaceArrayCompletionBlock)completion {
    
    NSMutableArray *responseCollection = [[NSMutableArray alloc] init];
    
    return [self startTaskWithHttpMethod:@"POST" path:@"detect"
                              parameters:@{@"url" : url}
                               urlParams:@{@"returnFaceId" : ObjectOrNull([self booleanToString:returnFaceId]),
                                           @"returnFaceLandmarks" : ObjectOrNull([self booleanToString:returnFaceLandmarks]),
                                           @"returnFaceAttributes" : ObjectOrNull([self faceAttributesWithArray:returnFaceAttributes])}
                                bodyData: nil
                              completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        if (!error && [responseObject isKindOfClass:[NSArray class]]) {
            
            for (NSDictionary *face in responseObject) {
                MPOFace *newFace = [[MPOFace alloc] initWithDictionary:face];
                [responseCollection addObject:newFace];
            }
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:responseCollection error:error];
    }];

}

- (NSURLSessionDataTask *)detectWithData:(NSData *)data returnFaceId:(BOOL)returnFaceId returnFaceLandmarks:(BOOL)returnFaceLandmarks returnFaceAttributes:(NSArray *)returnFaceAttributes completionBlock:(MPOFaceArrayCompletionBlock)completion {
    
    NSMutableArray *responseCollection = [[NSMutableArray alloc] init];
    
    return [self startTaskWithHttpMethod:@"POST" path:@"detect"
                              parameters:nil
                               urlParams:@{@"returnFaceId" : ObjectOrNull([self booleanToString:returnFaceId]),
                                           @"returnFaceLandmarks" : ObjectOrNull([self booleanToString:returnFaceLandmarks]),
                                           @"returnFaceAttributes" : ObjectOrNull([self faceAttributesWithArray:returnFaceAttributes])}
                                bodyData: data
                              completion:^(NSURLResponse *response, id responseObject, NSError *error) {
                                  
                                  if (!error && [responseObject isKindOfClass:[NSArray class]]) {
                                      
                                      for (NSDictionary *face in responseObject) {
                                          MPOFace *newFace = [[MPOFace alloc] initWithDictionary:face];
                                          [responseCollection addObject:newFace];
                                      }
                                  }
                                  
                                  [self runCompletionOnMainQueueWithBlock:completion object:responseCollection error:error];
                              }];
}

- (NSURLSessionDataTask *)verifyWithFirstFaceId:(NSString *)faceId1 faceId2:(NSString *)faceId2 completionBlock:(void (^) (MPOVerifyResult *verifyResult, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"POST" path:@"verify" parameters:@{@"faceId1" : faceId1, @"faceId2" : faceId2} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOVerifyResult *verifyResult = nil;
        
        if (!error && [responseObject isKindOfClass:[NSDictionary class]]) {
             verifyResult = [[MPOVerifyResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:verifyResult error:error];
    }];
}


//return IdentifyResult[]
- (NSURLSessionDataTask *)identifyWithPersonGroupId:(NSString *)personGroupId faceIds:(NSArray *)faceIds maxNumberOfCandidates:(NSInteger)maxNumberOfCandidates completionBlock:(MPOIdentifyResultArrayCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"POST" path:@"identify" parameters:@{@"faceIds" : faceIds, @"personGroupId" : personGroupId, @"maxNumOfCandidatesReturned" : [NSNumber numberWithInteger:maxNumberOfCandidates]} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        NSMutableArray *identifyResults = [[NSMutableArray alloc] init];
        
        if (!error && [responseObject isKindOfClass:[NSArray class]]) {
            
            for (id obj in responseObject) {
                MPOIdentifyResult *identifyResult = [[MPOIdentifyResult alloc] initWithDictionary:obj];
                [identifyResults addObject:identifyResult];
            }
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:identifyResults error:error];
    }];
}


//return SimilarFace[]
- (NSURLSessionDataTask *)findSimilarWithFaceId:(NSString *)faceId faceIds:(NSArray *)faceIds completionBlock:(MPOSimilarFaceArrayCompletionBlock)completion {

    return [self startTaskWithHttpMethod:@"POST" path:@"findsimilars" parameters:@{@"faceId" : faceId, @"faceIds" : faceIds} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        NSMutableArray *similarFaces = [[NSMutableArray alloc] init];

        if (!error) {
            for (id faceObj in responseObject) {
                NSString *faceId = faceObj[@"faceId"];
                
                MPOSimilarFace *similarFace = [[MPOSimilarFace alloc] init];
                similarFace.faceId = faceId;
                [similarFaces addObject:similarFace];
            }
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:similarFaces error:error];
    }];
}

//return GroupResult
- (NSURLSessionDataTask *)groupWithFaceIds:(NSArray *)faceIds completionBlock:(void (^) (MPOGroupResult *groupResult, NSError *error))completion {
    return [self startTaskWithHttpMethod:@"POST" path:@"group" parameters:@{@"faceIds" : faceIds} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOGroupResult *groupResult = nil;
        
        if (!error && [responseObject isKindOfClass:[NSDictionary class]]) {
            groupResult = [[MPOGroupResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:groupResult error:error];
    }];
    
}

#pragma mark PersonGroups
/*
 * =============================================================
 * ================ PersonGroups =================
 * =============================================================
*/

- (NSURLSessionDataTask *)createPersonGroupWithId:(NSString *)personGroupId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"PUT" path:[NSString stringWithFormat:@"persongroups/%@", personGroupId] parameters:@{@"name" : name, @"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)getPersonGroupWithPersonGroupId:(NSString *)personGroupId completionBlock:(void (^) (MPOPersonGroup *personGroup, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"persongroups/%@", personGroupId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
       
        MPOPersonGroup *personGroup = nil;
        
        if (!error) {
            personGroup = [[MPOPersonGroup alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:personGroup error:error];
    }];
}

- (NSURLSessionDataTask *)updatePersonGroupWithPersonGroupId:(NSString *)personGroupId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"PATCH" path:[NSString stringWithFormat:@"persongroups/%@", personGroupId] parameters:@{@"name" : name, @"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
       
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)deletePersonGroupWithPersonGroupId:(NSString *)personGroupId completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"DELETE" path:[NSString stringWithFormat:@"persongroups/%@", personGroupId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
       
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)getPersonGroupsWithCompletion:(MPOPersonGroupArrayCompletionBlock)completion {
    return [self startTaskWithHttpMethod:@"GET" path:@"persongroups" parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        NSMutableArray *responseCollection = [[NSMutableArray alloc] init];

        if (!error) {
            for (id obj in responseObject) {
                MPOPersonGroup *personGroup = [[MPOPersonGroup alloc] initWithDictionary:obj];
                [responseCollection addObject:personGroup];
            }
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:responseCollection error:error];
    }];
    
}

- (NSURLSessionDataTask *)trainPersonGroupWithPersonGroupId:(NSString *)personGroupId completionBlock:(MPOCompletionBlock)completion {
    return [self startTaskWithHttpMethod:@"POST" path:[NSString stringWithFormat:@"persongroups/%@/train", personGroupId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)getPersonGroupTrainingStatusWithPersonGroupId:(NSString *)personGroupId completionBlock:(void (^) (MPOTrainingStatus *trainingStatus, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"persongroups/%@/training", personGroupId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        MPOTrainingStatus *trainingStatus = nil;

        if (!error) {
            trainingStatus = [[MPOTrainingStatus alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:trainingStatus error:error];
    }];
    
}



#pragma mark Person
/*
 * =============================================================
 * ================ Person =================
 * =============================================================
 */

- (NSURLSessionDataTask *)createPersonWithPersonGroupId:(NSString *)personGroupId name:(NSString *)name userData:(NSString *)userData completionBlock:(void (^) (MPOCreatePersonResult *createPersonResult, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"POST" path:[NSString stringWithFormat:@"persongroups/%@/persons", personGroupId] parameters:@{@"name" : name, @"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {

        MPOCreatePersonResult *personResult = nil;
        
        if (!error) {
            personResult = [[MPOCreatePersonResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:personResult error:error];
    }];
    
}

//return Person
- (NSURLSessionDataTask *)getPersonWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId completionBlock:(void (^) (MPOPerson *person, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@", personGroupId, personId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
       
        MPOPerson *personResult = nil;
        
        if (!error) {
            personResult = [[MPOPerson alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:personResult error:error];
    }];
    
}

- (NSURLSessionDataTask *)updatePersonWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {

    return [self startTaskWithHttpMethod:@"PATCH" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@", personGroupId, personId] parameters:@{@"name" : name, @"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)deletePersonWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"DELETE" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@", personGroupId, personId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)getPersonsWithPersonGroupId:(NSString *)personGroupId completionBlock:(MPOPersonArrayCompletionBlock)completion {
 
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"persongroups/%@/persons", personGroupId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
       
        NSMutableArray *personCollection = [[NSMutableArray alloc] init];
        
        if (!error) {
            
            for (id obj in responseObject) {
                MPOPerson *person = [[MPOPerson alloc] initWithDictionary:obj];
                [personCollection addObject:person];
            }
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:personCollection error:error];
    }];
    
}

- (NSURLSessionDataTask *)getPersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId persistedFaceId:(NSString *)persistedFaceId completionBlock:(void (^) (MPOPersonFace *personFace, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@/persistedFaces/%@", personGroupId, personId, persistedFaceId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOPersonFace *personFace = nil;
        
        if (!error) {
            personFace = [[MPOPersonFace alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:personFace error:error];
    }];
    
}

- (NSURLSessionDataTask *)updatePersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId persistedFaceId:(NSString *)persistedFaceId userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"PATCH" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@/persistedFaces/%@", personGroupId, personId, persistedFaceId] parameters:@{@"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

- (NSURLSessionDataTask *)deletePersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId persistedFaceId:(NSString *)persistedFaceId completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"DELETE" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@/persistedFaces/%@", personGroupId, personId, persistedFaceId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
    
}


- (NSURLSessionDataTask *)addPersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId url:(NSString *)url userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion {

    return [self startTaskWithHttpMethod:@"POST" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@/persistedFaces", personGroupId, personId] parameters:@{@"url" : url} urlParams:@{@"userData" : ObjectOrNull(userData), @"targetFace" : ObjectOrNull([self faceRectangleStringRepresentation:faceRectangle])} bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOAddPersistedFaceResult *addPersistedFaceResult = nil;
        
        if (!error) {
            addPersistedFaceResult = [[MPOAddPersistedFaceResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:addPersistedFaceResult error:error];
    }];
    
}

- (NSURLSessionDataTask *)addPersonFaceWithPersonGroupId:(NSString *)personGroupId personId:(NSString *)personId data:(NSData *)data userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"POST" path:[NSString stringWithFormat:@"persongroups/%@/persons/%@/persistedFaces", personGroupId, personId] parameters:nil urlParams:@{@"userData" : ObjectOrNull(userData), @"targetFace" : ObjectOrNull([self faceRectangleStringRepresentation:faceRectangle])} bodyData:data completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOAddPersistedFaceResult *addPersistedFaceResult = nil;
        
        if (!error) {
            addPersistedFaceResult = [[MPOAddPersistedFaceResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:addPersistedFaceResult error:error];
    }];
}

#pragma mark FaceLists
/*
 * =============================================================
 * ================ FaceLists =================
 * =============================================================
 */
//return success
- (NSURLSessionDataTask *)createFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"PUT" path:[NSString stringWithFormat:@"facelists/%@", faceListId] parameters:@{@"name" : name, @"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

//return FaceList
- (NSURLSessionDataTask *)getFaceListWithFaceListId:(NSString *)faceListId completionBlock:(void (^) (MPOFaceList *addPersistedFaceResult, NSError *error))completion {
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"facelists/%@", faceListId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOFaceList *faceList = nil;
        
        if (!error) {
            faceList = [[MPOFaceList alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:faceList error:error];
    }];
}

//return FaceListMetaData[]
- (NSURLSessionDataTask *)listFaceListsWithCompletion:(MPOFaceListMetadataArrayCompletionBlock)completion {
    
    return [self startTaskWithHttpMethod:@"GET" path:[NSString stringWithFormat:@"facelists"] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        NSMutableArray *faceList = [[NSMutableArray alloc] init];
        
        if (!error) {
            
            for (id obj in responseObject) {
                MPOFaceListMetadata *faceListMetadata = [[MPOFaceListMetadata alloc] initWithDictionary:obj];
                [faceList addObject:faceListMetadata];
            }
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:faceList error:error];

        
    }];
}

//return success
- (NSURLSessionDataTask *)updateFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {
    return [self startTaskWithHttpMethod:@"PATCH" path:[NSString stringWithFormat:@"facelists/%@", faceListId] parameters:@{@"name" : name, @"userData" : ObjectOrNull(userData)} urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

//return success
- (NSURLSessionDataTask *)deleteFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name userData:(NSString *)userData completionBlock:(MPOCompletionBlock)completion {
    return [self startTaskWithHttpMethod:@"DELETE" path:[NSString stringWithFormat:@"facelists/%@", faceListId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

//return success
- (NSURLSessionDataTask *)deleteFacesFromFaceListWithFaceListId:(NSString *)faceListId name:(NSString *)name persistedFaceId:(NSString *)persistedFaceId completionBlock:(MPOCompletionBlock)completion {
    return [self startTaskWithHttpMethod:@"DELETE" path:[NSString stringWithFormat:@"facelists/%@/persistedFaces/%@", faceListId, persistedFaceId] parameters:nil urlParams:nil bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        [self runCompletionOnMainQueueWithBlock:completion error:error];
    }];
}

//return AddPersistedFaceResult
- (NSURLSessionDataTask *)addFacesToFaceListWithFaceListId:(NSString *)faceListId url:(NSString *)url userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"POST" path:[NSString stringWithFormat:@"facelists/%@/persistedFaces", faceListId] parameters:@{@"url" : url} urlParams: @{@"userData" : ObjectOrNull(userData), @"targetFace" : ObjectOrNull([self faceRectangleStringRepresentation:faceRectangle])} bodyData:nil completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOAddPersistedFaceResult *addPersistedFaceResult = nil;
        
        if (!error) {
            addPersistedFaceResult = [[MPOAddPersistedFaceResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:addPersistedFaceResult error:error];
    }];
}

//return AddPersistedFaceResult
- (NSURLSessionDataTask *)addFacesToFaceListWithFaceListId:(NSString *)faceListId data:(NSData *)data userData:(NSString *)userData faceRectangle:(MPOFaceRectangle *)faceRectangle completionBlock:(void (^) (MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error))completion {
    
    return [self startTaskWithHttpMethod:@"POST" path:[NSString stringWithFormat:@"facelists/%@/persistedFaces", faceListId] parameters:nil urlParams: @{@"userData" : ObjectOrNull(userData), @"targetFace": ObjectOrNull([self faceRectangleStringRepresentation:faceRectangle])} bodyData:data completion:^(NSURLResponse *response, id responseObject, NSError *error) {
        
        MPOAddPersistedFaceResult *addPersistedFaceResult = nil;
        
        if (!error) {
            addPersistedFaceResult = [[MPOAddPersistedFaceResult alloc] initWithDictionary:responseObject];
        }
        
        [self runCompletionOnMainQueueWithBlock:completion object:addPersistedFaceResult error:error];
    }];
}


#pragma mark Helpers

/*
 * =============================================================
 * ================ Helpers =================
 * =============================================================
 */

- (NSURLSessionDataTask *)startTaskWithHttpMethod:(NSString *)httpMethod path:(NSString *)path parameters:(NSDictionary *)params urlParams:(NSDictionary *)urlParams bodyData:(NSData *)bodyData completion:(PORequestCompletionBlock)completion {
    
    NSString *basePath = @"https://api.projectoxford.ai/face/v1.0/";
    
    NSURLSessionConfiguration *config = [NSURLSessionConfiguration defaultSessionConfiguration];
    
    if (!bodyData) {
        config.HTTPAdditionalHeaders = @{
                                         @"Ocp-Apim-Subscription-Key" : self.subscriptionKey,
                                         @"Content-Type" : @"application/json"
                                         };
    }
    else {
        config.HTTPAdditionalHeaders = @{
                                         @"Ocp-Apim-Subscription-Key" : self.subscriptionKey,
                                         @"Content-Type" : @"application/octet-stream"
                                         };
    }
 
    NSURLSession *session = [NSURLSession sessionWithConfiguration:config];

    
    //build the queryString
    
    NSMutableString *queryString = [[NSMutableString alloc] initWithString:@""];
    
    for (NSString* key in urlParams) {
        id value = [urlParams objectForKey:key];
        
        if (![value isKindOfClass:[NSNull class]]) {
            if ([queryString isEqualToString:@""]) {
                [queryString appendString:[NSString stringWithFormat:@"?%@=%@", key, value]];
            }
            else {
                [queryString appendString:[NSString stringWithFormat:@"&%@=%@", key, value]];
            }
        }
    }
    
   
    NSMutableURLRequest *request = [[NSMutableURLRequest alloc] initWithURL:[NSURL URLWithString:[NSString stringWithFormat:@"%@%@%@", basePath, path, queryString]]];
    
    request.HTTPMethod = httpMethod;

    //if there is no body data and params is not nil, assume we have JSON to encode for the body
    if (params && !bodyData) {
        NSError *error = nil;
        NSData *data = [NSJSONSerialization dataWithJSONObject:params
                                                       options:kNilOptions error:&error];
        request.HTTPBody = data;
    }
    //otherwise set the httpBody to bodyData (used mostly in detection API when passing NSData, not url)
    else if (bodyData) {
        request.HTTPBody = bodyData;
    }
    
    NSURLSessionDataTask *dataTask = [session dataTaskWithRequest:request completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
        
        NSHTTPURLResponse *httpResponse = (NSHTTPURLResponse *) response;
        NSInteger responseStatusCode = httpResponse.statusCode;

        if (responseStatusCode >= 200 && responseStatusCode <= 299) {
            
            NSString *contentTypeHeader = httpResponse.allHeaderFields[@"Content-Type"];
            
            //if the content type is JSON, attempt to parse it
            if (contentTypeHeader && [contentTypeHeader rangeOfString:@"application/json"].location != NSNotFound) {
                NSError *jsonParseError;
                NSDictionary *jsonDict = [NSJSONSerialization JSONObjectWithData:data options:0 error:&jsonParseError];
                if (!jsonParseError) {
                    completion(response, jsonDict, error);
                }
                else {
                    [NSException raise:@"Failed to parse json" format:@"jsonParse error"];
                }
            }
            //if we do not have json to decode, call the completion block
            else {
                completion(response, data, error);
            }
            
        }
        else {
            
            NSString *responseAsString = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
            
            NSError *error = [NSError errorWithDomain:[NSString stringWithFormat:@"POFaceServiceClient error - http response is not success : %@", responseAsString]
                                                 code:responseStatusCode
                                             userInfo:nil];
            completion(response, nil, error);
        }
        
    }];
    
    [dataTask resume];
    

    return dataTask;
}

- (void)runCompletionOnMainQueueWithBlock:(void (^) (id obj, NSError *error))completionBlock object:(id )object error:(NSError *)error {

    dispatch_async(dispatch_get_main_queue(), ^{
        completionBlock(object, error);
    });
}

- (void)runCompletionOnMainQueueWithBlock:(void (^) (NSError *error))completionBlock error:(NSError *)error {
    
    dispatch_async(dispatch_get_main_queue(), ^{
        completionBlock(error);
    });
}

- (NSString *)booleanToString:(BOOL)boolean {
    
    if (boolean) {
        return @"true";
    }
    else {
        return @"false";
    }
}

static id ObjectOrNull(id object)
{
    return object ?: [NSNull null];
}

@end
