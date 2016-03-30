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
#import "MPOTestHelpers.h"
@interface GroupTestCase : XCTestCase
@property NSDictionary *testDataDict;
@end

@implementation GroupTestCase

- (void)setUp {
    [super setUp];
    // Put setup code here. This method is called before the invocation of each test method in the class.
    
    self.testDataDict = [MPOTestHelpers detectWithDict:@{
                                                         @"chris1": kChrisImageName1,
                                                         @"chris2": kChrisImageName2,
                                                         @"chris3": kChrisImageName3,
                                                         @"alberto1": kAlbertoImageName1,
                                                         @"alberto2": kAlbertoImageName2,
                                                         @"john1": kJohnImageName1,
                                                         }];
}

- (void)tearDown {
    // Put teardown code here. This method is called after the invocation of each test method in the class.
    [super tearDown];
}

- (void)testGroup {
    
    XCTestExpectation *expectation = [self expectationWithDescription:@"asynchronous request"];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    NSArray *faceIds = [self.testDataDict allValues];

    [client groupWithFaceIds:faceIds completionBlock:^(MPOGroupResult *groupResult, NSError *error) {
       
        if (error) {
            XCTFail("fail");
        }
        else {
            
            XCTAssertEqual(groupResult.groups.count, 2);
            XCTAssertEqual(groupResult.messeyGroup.count, 1);

            for (NSArray *group in groupResult.groups) {
                
                if (group.count == 3) {
                    //chris
                    XCTAssertTrue([group containsObject:self.testDataDict[@"chris1"]]);
                    XCTAssertTrue([group containsObject:self.testDataDict[@"chris2"]]);
                    XCTAssertTrue([group containsObject:self.testDataDict[@"chris3"]]);
                    
                }
                else if (group.count == 2) {
                    //alberto
                    XCTAssertTrue([group containsObject:self.testDataDict[@"alberto1"]]);
                    XCTAssertTrue([group containsObject:self.testDataDict[@"alberto2"]]);
                }
                
            }
            
            //michelle1 should be in messeyGroup
            XCTAssertTrue([groupResult.messeyGroup containsObject:self.testDataDict[@"john1"]]);
            
        }
        
        [expectation fulfill];
        
    }];

    [self waitForExpectationsWithTimeout:10.0 handler:nil];
    
}
@end
