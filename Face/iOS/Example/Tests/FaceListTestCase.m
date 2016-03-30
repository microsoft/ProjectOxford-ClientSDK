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
#import "MPOFaceSDK.h"
#import "MPOTestConstants.h"
#define kFaceListName @"testfacelistname"

@interface FaceListTestCase : XCTestCase

@end

@implementation FaceListTestCase

- (void)setUp {
    [super setUp];
    
    // Put setup code here. This method is called before the invocation of each test method in the class.
}

- (void)tearDown {
    // Put teardown code here. This method is called after the invocation of each test method in the class.
    
    __block BOOL finished = NO;
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
        
    [client deleteFaceListWithFaceListId:kFaceListName name:@"chrisfacelistname" userData:@"chrisfacelistuserdata" completionBlock:^(NSError *error) {
        
        if (error) {
            NSLog(@"error in teardown");
        }
        else {
            finished = YES;
        }
        
    }];
    
    while (!finished) {
        [[NSRunLoop currentRunLoop] runMode:NSDefaultRunLoopMode beforeDate:[NSDate distantFuture]];
    }
    
    [super tearDown];
}

- (void)testCreateFaceList {
    XCTestExpectation *expectation = [self expectationWithDescription:@"asynchronous request"];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client createFaceListWithFaceListId:kFaceListName name:@"chrisfacelistname" userData:@"chrisfacelistuserdata" completionBlock:^(NSError *error) {
       
        if (error) {
            XCTFail();
        }
        else {
            [self addFirstFaceToFaceList:expectation];
        }
        
    }];
    
    [self waitForExpectationsWithTimeout:20.0 handler:nil];

}

- (void)addFirstFaceToFaceList:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client addFacesToFaceListWithFaceListId:kFaceListName data:UIImageJPEGRepresentation([UIImage imageNamed:kChrisImageName1], 1.0) userData:@"chrisfacelistuserdata1" faceRectangle:nil completionBlock:^(MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error) {
       
        if (error) {
            XCTFail();
        }
        else {
            [self addSecondFaceToFaceList:expectation];
        }
        
        
    }];
    
}

- (void)addSecondFaceToFaceList:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client addFacesToFaceListWithFaceListId:kFaceListName data:UIImageJPEGRepresentation([UIImage imageNamed:kChrisImageName2], 1.0) userData:@"chrisfacelistuserdata2" faceRectangle:nil completionBlock:^(MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error) {
        
        if (error) {
            XCTFail();
        }
        else {
            [self listFaceLists:expectation];
        }
    }];
    
}


- (void)listFaceLists:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client listFaceListsWithCompletion:^(NSArray<MPOFaceListMetadata *> *collection, NSError *error) {
        
        if (error) {
            XCTFail();
        }
        else {
            
            XCTAssertEqual(collection.count, 1);
            
            for (MPOFaceListMetadata *faceListMetadata in collection) {
                XCTAssertEqualObjects(faceListMetadata.faceListId, kFaceListName);
            }
            
            [self getFaceList:expectation];
        }
        
    }];
    
    
}


- (void)getFaceList:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    [client getFaceListWithFaceListId:kFaceListName completionBlock:^(MPOFaceList *faceList, NSError *error) {
       
        if (error) {
            XCTFail();
        }
        else {
            
            XCTAssertEqualObjects(faceList.faceListId, kFaceListName);
            
            XCTAssertEqual(faceList.persistedFaces.count, 2);

            for (MPOFaceMetadata *faceMetadata in faceList.persistedFaces) {
                XCTAssertTrue([faceMetadata.userData containsString:@"chrisfacelistuserdata"]);
            }
            
            [expectation fulfill];
        }
        
    }];
    
}

@end
