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
@interface VerifyTestCase : XCTestCase
@property NSString *testFirstFaceId;
@property NSString *testSecondFaceId;
@end

@implementation VerifyTestCase

- (void)setUp {
    [super setUp];
    // Put setup code here. This method is called before the invocation of each test method in the class.
    
}

- (void)tearDown {
    // Put teardown code here. This method is called after the invocation of each test method in the class.
    [super tearDown];
}


- (void)testVerify {
    XCTestExpectation *expectation = [self expectationWithDescription:@"asynchronous request"];
    
    [self detectFirstImage:expectation];
    
    [self waitForExpectationsWithTimeout:10.0 handler:nil];

}

- (void)detectFirstImage:(XCTestExpectation *)expectation {
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client detectWithData:UIImageJPEGRepresentation([UIImage imageNamed:kChrisImageName1], 1.0) returnFaceId:YES returnFaceLandmarks:NO returnFaceAttributes:nil completionBlock:^(NSArray<MPOFace *> *collection, NSError *error) {
       
        if (error) {
            XCTFail(@"fail");
        }
        else {
            XCTAssertEqual(collection.count, 1);
            
            for (MPOFace *face in collection) {
                self.testFirstFaceId = face.faceId;
            }
            
        }
        
        [self detectSecondImage:expectation];
        
    }];
    
}

- (void)detectSecondImage:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    [client detectWithData:UIImageJPEGRepresentation([UIImage imageNamed:kChrisImageName2], 1.0) returnFaceId:YES returnFaceLandmarks:NO returnFaceAttributes:nil completionBlock:^(NSArray<MPOFace *> *collection, NSError *error) {
        
        if (error) {
            XCTFail(@"fail");
        }
        else {
            XCTAssertEqual(collection.count, 1);
            
            for (MPOFace *face in collection) {
                self.testSecondFaceId = face.faceId;
            }
            
        }
        
        [self startVerify:expectation];

    }];
}


- (void)startVerify:(XCTestExpectation *)expectation {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
  
    [client verifyWithFirstFaceId:self.testFirstFaceId faceId2:self.testSecondFaceId completionBlock:^(MPOVerifyResult *verifyResult, NSError *error) {
       
        if (error) {
            XCTFail("fail");
        }
        else {
            XCTAssertTrue(verifyResult.isIdentical);
        }
        
        [expectation fulfill];
        
    }];
}


@end
