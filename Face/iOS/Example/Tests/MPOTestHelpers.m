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

#import "MPOTestHelpers.h"
#import "MPOFaceSDK.h"
#import "MPOTestConstants.h"
#import <UIKit/UIKit.h>

@implementation MPOTestHelpers

+ (NSDictionary *)detectWithDict:(NSDictionary *)dataDict {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];

    NSMutableDictionary *testDataDict = [[NSMutableDictionary alloc] init];

    for (NSString *key in dataDict) {
        [client detectWithData:UIImageJPEGRepresentation([UIImage imageNamed:[dataDict objectForKey:key]], 1.0) returnFaceId:YES returnFaceLandmarks:NO returnFaceAttributes:nil completionBlock:^(NSArray<MPOFace *> *collection, NSError *error) {
            if (error) {
                [NSException raise:@"Error in detection" format:@"Error: %@", error];
                
            }
            else {
                
                for (MPOFace *face in collection) {
                    [testDataDict setObject:face.faceId forKey:key];
                }
            }
        }];
        
    }
    
    NSDate *loopUntil = [NSDate dateWithTimeIntervalSinceNow:10];
    while ([[testDataDict allKeys] count] != [[dataDict allKeys] count] && [loopUntil timeIntervalSinceNow] > 0) {
        [[NSRunLoop currentRunLoop] runMode:NSDefaultRunLoopMode
                                 beforeDate:loopUntil];
    }
    
    if ([[testDataDict allKeys] count] != [[dataDict allKeys] count])
    {
        [NSException raise:@"Setup has failed" format:@""];

    }
    
    [NSThread sleepForTimeInterval:3.0f];

    return testDataDict;
}

+ (BOOL)addMultiplePersonFaces:(NSArray *)urlArray personGroupId:(NSString *)personGroupId personId:(NSString *)personId {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    dispatch_group_t taskGroup = dispatch_group_create();

    __block int facesAdded = 0;
    
    for (NSString *url in urlArray) {
        dispatch_group_enter(taskGroup);

        [client addPersonFaceWithPersonGroupId:personGroupId personId:personId data:UIImageJPEGRepresentation([UIImage imageNamed:url], 1.0) userData:@"someData" faceRectangle:nil completionBlock:^(MPOAddPersistedFaceResult *addPersistedFaceResult, NSError *error) {
            
            if (error) {
                [NSException raise:@"Error in detection" format:@"Error: %@", error];
            }
            else {
                dispatch_group_leave(taskGroup);
                
                facesAdded++;
            }

        }];
        
    }
    
    // Waiting for threads
    if (dispatch_group_wait(taskGroup, dispatch_time(DISPATCH_TIME_NOW, 10000000000)) != 0) {
    
    }
    return YES;
}

+ (BOOL)clearAllPersonGroups {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:kOxfordApiKey];
    
    __block BOOL hasSetupCompleted = NO;
    
    
    [client getPersonGroupsWithCompletion:^(NSArray<MPOPersonGroup *> *collection, NSError *error) {
        
        if (error) {
            [NSException raise:@"Error: failed getting person groups" format:@"Error: %@", error];
        }
        
        else {
            
            if ([collection count] > 0) {
                
                for (MPOPersonGroup *personGroup in collection) {
                    
                    [client deletePersonGroupWithPersonGroupId:personGroup.personGroupId completionBlock:^(NSError *error) {
                        
                        if (error) {
                            [NSException raise:@"failed clearing person group" format:@"Error: %@", error];

                        }
                        else {
                            
                            hasSetupCompleted = YES;
                            
                        }
                        
                    }];
                    
                }

            }
            else {
                hasSetupCompleted = YES;
            }
        }
        
    }];
    
    
    NSDate *loopUntil = [NSDate dateWithTimeIntervalSinceNow:10];
    while (hasSetupCompleted == NO && [loopUntil timeIntervalSinceNow] > 0) {
        [[NSRunLoop currentRunLoop] runMode:NSDefaultRunLoopMode
                                 beforeDate:loopUntil];
    }
    
    if (!hasSetupCompleted)
    {
        [NSException raise:@"Setup has failed" format:@""];
    }

    [NSThread sleepForTimeInterval:3.0f];
    
    return hasSetupCompleted;
}

@end
