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

#import <XCTest/XCTest.h>
#import "MPOTestConstants.h"
#import "MPOTestHelpers.h"
#import "MPOFaceSDK.h"
@interface IdentifyTestCase : XCTestCase
@property NSDictionary *testDataDict;
@property NSTimer *waitingTimer;
@property NSMutableDictionary *peopleDataDict;

@end

#define kPersonGroupId @"persongroup"
#define kPersonGroupName @"persongroupname"
#define kPersonGroupUserData @"persongroupuserdata"

@implementation IdentifyTestCase

- (void)setUp {
    [super setUp];
    // Put setup code here. This method is called before the invocation of each test method in the class.
    
    [MPOTestHelpers clearAllPersonGroups];
    
    self.peopleDataDict = [[NSMutableDictionary alloc] init];
    
    self.testDataDict = [MPOTestHelpers detectWithDict:@{
                                                         @"chris1": kChrisImageName1,
                                                         @"chris2": kChrisImageName2,
                                                         @"chris3": kChrisImageName3,
                                                         @"alberto1": kAlbertoImageName1,
                                                         @"alberto2": kAlbertoImageName2,
                                                         @"john1": kJohnImageName1,
                                                         @"john2": kJohnImageName2,
                                                         }];
}

- (void)tearDown {
    // Put teardown code here. This method is called after the invocation of each test method in the class.
    [super tearDown];
}

- (void)testIdentification {

    XCTestExpectation *expectation = [self expectationWithDescription:@"asynchronous request"];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client createPersonGroupWithId:kPersonGroupId name:kPersonGroupName userData:kPersonGroupUserData completionBlock:^(NSError *error) {
       
        if (error) {
            XCTFail("fail");
        }
        else {
            [self createChrisPerson:expectation];
        }
        
    }];
    
    [self waitForExpectationsWithTimeout:120.0 handler:nil];
}


- (void)createChrisPerson:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client createPersonWithPersonGroupId:kPersonGroupId name:@"chris" userData:@"chris_userdata" completionBlock:^(MPOCreatePersonResult *createPersonResult, NSError *error) {
        if (error) {
            XCTFail("fail");
        }
        else {
            [self.peopleDataDict setObject:createPersonResult.personId forKey:@"chris"];

            [MPOTestHelpers addMultiplePersonFaces:@[kChrisImageName1, kChrisImageName2, kChrisImageName3] personGroupId:kPersonGroupId personId:createPersonResult.personId];
            
            [self createAlbertoPerson:expectation];
        }

    }];
}


- (void)createAlbertoPerson:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client createPersonWithPersonGroupId:kPersonGroupId name:@"alberto" userData:@"alberto_userdata" completionBlock:^(MPOCreatePersonResult *createPersonResult, NSError *error) {
        if (error) {
            XCTFail("fail");
        }
        else {
            [self.peopleDataDict setObject:createPersonResult.personId forKey:@"alberto"];
            
            [MPOTestHelpers addMultiplePersonFaces:@[kAlbertoImageName1, kAlbertoImageName2] personGroupId:kPersonGroupId personId:createPersonResult.personId];
            
            [self createJohnPerson:expectation];
        }
        
    }];
}

- (void)createJohnPerson:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client createPersonWithPersonGroupId:kPersonGroupId name:@"john" userData:@"john_userdata" completionBlock:^(MPOCreatePersonResult *createPersonResult, NSError *error) {
        if (error) {
            XCTFail("fail");
        }
        else {
            [self.peopleDataDict setObject:createPersonResult.personId forKey:@"john"];
            
            [MPOTestHelpers addMultiplePersonFaces:@[kJohnImageName1, kJohnImageName2] personGroupId:kPersonGroupId personId:createPersonResult.personId];
            
            [self trainGroup:expectation];
            
        }
        
    }];

}

- (void)trainGroup:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client trainPersonGroupWithPersonGroupId:kPersonGroupId completionBlock:^(NSError *error) {
        if (error) {
            XCTFail("fail");
        }
        else {
            [self checkTrainingStatus:expectation];
        }

    }];
    
}
- (void)checkTrainingStatus:(XCTestExpectation *)expectation {
    self.waitingTimer = [NSTimer scheduledTimerWithTimeInterval:3.0 target:self selector:@selector(trainingStatusCheck:) userInfo:expectation repeats: YES];
}

- (void)trainingStatusCheck:(NSTimer *)timer
{
    XCTestExpectation *expectation = timer.userInfo;
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client getPersonGroupTrainingStatusWithPersonGroupId:kPersonGroupId completionBlock:^(MPOTrainingStatus *trainingStatus, NSError *error) {
        
        if (error) {
            XCTFail("fail");
        }
        else {
            if ([trainingStatus.status isEqualToString:@"succeeded"]) {
                
                [self.waitingTimer invalidate];
                
                [self identify:expectation];
            }
            
        }
        
    }];
    
}

- (void)identify:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client identifyWithPersonGroupId:kPersonGroupId faceIds:[[NSArray alloc] initWithObjects:self.testDataDict[@"chris3"], self.testDataDict[@"alberto2"], nil] maxNumberOfCandidates:4 completionBlock:^(NSArray *collection, NSError *error) {
        
        if (error) {
            XCTFail("fail");
        }
        else {
            XCTAssertEqual(collection.count, 2);

            for (MPOIdentifyResult *result in collection) {

                if ([result.faceId isEqualToString:self.testDataDict[@"chris3"]]) {
                    
                    for (MPOCandidate *candidate in result.candidates) {
                        XCTAssertEqualObjects(candidate.personId, self.peopleDataDict[@"chris"]);
                    }
                }
                else if ([result.faceId isEqualToString:self.testDataDict[@"alberto2"]]) {
                    
                    for (MPOCandidate *candidate in result.candidates) {
                        XCTAssertEqualObjects(candidate.personId, self.peopleDataDict[@"alberto"]);
                    }
                }
            }
            
        }
        
        [expectation fulfill];
        
    }];
    
}
@end
