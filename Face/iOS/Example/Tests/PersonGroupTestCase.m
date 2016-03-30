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
#import "MPOFaceSDK.h"
#import "MPOTestHelpers.h"
@interface PersonGroupTestCase : XCTestCase

@end

@implementation PersonGroupTestCase

- (void)setUp {
    [super setUp];
    // Put setup code here. This method is called before the invocation of each test method in the class.
    
    [MPOTestHelpers clearAllPersonGroups];
}

- (void)tearDown {
    // Put teardown code here. This method is called after the invocation of each test method in the class.
    [super tearDown];
}

- (void)testPersonGroup {
    

    XCTestExpectation *expectation = [self expectationWithDescription:@"asynchronous request"];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client createPersonGroupWithId:@"test_persongroup_id" name:@"test_persongroup_name" userData:@"test_persongroup_userdata" completionBlock:^(NSError *error) {
       
        if (error) {
            XCTFail("error creating test person group");
        }
        else {
            [self getPersonGroup:expectation];
        }
        
        
    }];
    
    [self waitForExpectationsWithTimeout:10.0 handler:nil];
    
}

- (void)getPersonGroup:(XCTestExpectation *)expectation {
 
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client getPersonGroupsWithCompletion:^(NSArray<MPOPersonGroup *> *collection, NSError *error) {
       
        if (error) {
            XCTFail("error");
        }
        else {
            XCTAssertEqual(collection.count, 1);

            for (MPOPersonGroup *personGroup in collection) {

                XCTAssertEqualObjects(personGroup.personGroupId, @"test_persongroup_id");
                XCTAssertEqualObjects(personGroup.name, @"test_persongroup_name");
                XCTAssertEqualObjects(personGroup.userData, @"test_persongroup_userdata");
            }
            
        }
        
        [self updatePersonGroup:expectation];
        
    }];
    
}


- (void)updatePersonGroup:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client updatePersonGroupWithPersonGroupId:@"test_persongroup_id" name:@"test_persongroup_name_update" userData:@"test_persongroup_userdata_update" completionBlock:^(NSError *error) {
       
        if (error) {
            XCTFail("error");
        }
        
        [self checkIfUpdateWorked:expectation];
        
    }];
    
    
}

- (void)checkIfUpdateWorked:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client getPersonGroupsWithCompletion:^(NSArray *collection, NSError *error) {
        
        if (error) {
            XCTFail("error");
        }
        else {
            XCTAssertEqual(collection.count, 1);
            
            for (MPOPersonGroup *personGroup in collection) {
                
                XCTAssertEqualObjects(personGroup.personGroupId, @"test_persongroup_id");
                XCTAssertEqualObjects(personGroup.name, @"test_persongroup_name_update");
                XCTAssertEqualObjects(personGroup.userData, @"test_persongroup_userdata_update");
            }
            
        }
        
        [self deletePersonGroup:expectation];
        
    }];

    
}

- (void)deletePersonGroup:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client deletePersonGroupWithPersonGroupId:@"test_persongroup_id" completionBlock:^(NSError *error) {
        if (error) {
            XCTFail("error");
        }
        [self checkIfDeleteWorked:expectation];

    }];
    
    
}
- (void)checkIfDeleteWorked:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client getPersonGroupsWithCompletion:^(NSArray *collection, NSError *error) {
        
        if (error) {
            XCTFail("error");
        }
        else {
            XCTAssertEqual(collection.count, 0);
        }
        
        [expectation fulfill];
    }];

    
}

@end
