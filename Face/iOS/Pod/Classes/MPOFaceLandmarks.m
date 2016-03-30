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

#import "MPOFaceLandmarks.h"

@implementation MPOFaceLandmarks
-(instancetype)initWithDictionary:(NSDictionary *)dict {
    self = [super init];
    if (self) {
        self.pupilLeft = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"pupilLeft"]];
        self.pupilRight = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"pupilRight"]];
        self.noseTip = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"noseTip"]];
        self.mouthLeft = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"mouthLeft"]];
        self.mouthRight = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"mouthRight"]];
        self.eyebrowLeftOuter = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyebrowLeftOuter"]];
        self.eyebrowLeftInner = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyebrowLeftInner"]];
        self.eyeLeftOuter = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeLeftOuter"]];
        self.eyeLeftTop = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeLeftTop"]];
        self.eyeLeftBottom = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeLeftBottom"]];
        self.eyeLeftInner = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeLeftInner"]];
        self.eyebrowRightInner = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyebrowRightInner"]];
        self.eyeRightInner = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeRightInner"]];
        self.eyeRightTop = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeRightTop"]];
        self.eyeRightBottom = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeRightBottom"]];
        self.eyeRightOuter = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"eyeRightOuter"]];
        self.noseRootLeft = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"noseRootLeft"]];
        self.noseRootRight = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"noseRootRight"]];
        self.noseLeftAlarTop = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"noseLeftAlarTop"]];
        self.noseLeftAlarOutTip = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"noseLeftAlarOutTip"]];
        self.noseRightAlarOutTip = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"noseRightAlarOutTip"]];
        self.upperLipTop = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"upperLipTop"]];
        self.upperLipBottom = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"upperLipBottom"]];
        self.underLipBottom = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"underLipBottom"]];
        self.underLipTop = [[MPOFaceFeatureCoordinate alloc] initWithDictionary:dict[@"underLipTop"]];
    }
    return self;
}
@end
