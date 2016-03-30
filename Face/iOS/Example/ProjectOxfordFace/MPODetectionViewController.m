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

#import "MPODetectionViewController.h"
#import "MPODetectionCollectionViewCell.h"
#import <ProjectOxfordFace/MPOFaceServiceClient.h>
#import "MPOUtilities.h"
#import "MPODemoConstants.h"
#import "MPOActivityIndicatorViewController.h"
@interface MPODetectionFaceCellObject : NSObject
@property (nonatomic, strong) UIImage *croppedFaceImage;
@property (nonatomic, strong) NSString *ageText;
@property (nonatomic, strong) NSString *genderText;
@property (nonatomic, strong) NSString *headPoseText;
@property (nonatomic, strong) NSString *moustacheText;
@property (nonatomic, strong) NSString *smileText;
@end

@implementation MPODetectionFaceCellObject
@end

@interface MPODetectionViewController ()
@property (nonatomic, strong) NSMutableArray *faceCellObjects;
@end

@implementation MPODetectionViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    self.collectionView.dataSource = self;
    self.collectionView.delegate = self;
    
    self.faceCellObjects = [[NSMutableArray alloc] init];
    
    // Do any additional setup after loading the view.
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}
/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

- (IBAction)selectImageButtonPressed:(id)sender {
    
    UIAlertController *actionSheet = [UIAlertController alertControllerWithTitle:@"Select a photo" message:nil preferredStyle:UIAlertControllerStyleActionSheet];
    
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    picker.delegate = self;
    picker.allowsEditing = YES;
    picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;

    
    [actionSheet addAction:[UIAlertAction actionWithTitle:@"Cancel" style:UIAlertActionStyleCancel handler:^(UIAlertAction *action) {
        [self dismissViewControllerAnimated:YES completion:nil];
    }]];
    
    [actionSheet addAction:[UIAlertAction actionWithTitle:@"Use Camera" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        picker.sourceType = UIImagePickerControllerSourceTypeCamera;
        [self dismissViewControllerAnimated:YES completion:nil];
        [self presentViewController:picker animated:YES completion:nil];

    }]];
    
    [actionSheet addAction:[UIAlertAction actionWithTitle:@"Use Gallery" style:UIAlertActionStyleDefault handler:^(UIAlertAction *action) {
        picker.sourceType = UIImagePickerControllerSourceTypePhotoLibrary;
        [self dismissViewControllerAnimated:YES completion:nil];
        [self presentViewController:picker animated:YES completion:nil];

    }]];
    
    // Present action sheet.
    [self presentViewController:actionSheet animated:YES completion:nil];

}
- (void)runDetection {
    
    //remove any existing faces, if we have run detection previously
    [self.faceCellObjects removeAllObjects];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:ProjectOxfordFaceSubscriptionKey];
    
    NSData *data = UIImageJPEGRepresentation(self.imageView.image, 0.8);

    //show loading indicator
    MPOActivityIndicatorViewController *indicatorViewController = [[MPOActivityIndicatorViewController alloc] init];
    [self presentViewController:indicatorViewController animated:YES completion:nil];
    
    [client detectWithData:data returnFaceId:YES returnFaceLandmarks:YES returnFaceAttributes:@[@(MPOFaceAttributeTypeAge), @(MPOFaceAttributeTypeFacialHair), @(MPOFaceAttributeTypeHeadPose), @(MPOFaceAttributeTypeSmile), @(MPOFaceAttributeTypeGender)] completionBlock:^(NSArray<MPOFace *> *collection, NSError *error) {
        
        if (error) {
            NSLog(@"Error: %@", error);
        }
        else {
            self.numberOfFacesDetectedLabel.text = [NSString stringWithFormat:@"%@ face detected", @(collection.count)];
            
            for (MPOFace *face in collection) {
                UIImage *croppedImage = [MPOUtilities cropImageToFaceRectangle:self.imageView.image faceRectangle:face.faceRectangle];
                
                MPODetectionFaceCellObject *obj = [[MPODetectionFaceCellObject alloc] init];
                obj.croppedFaceImage = croppedImage;
                obj.ageText = [NSString stringWithFormat:@"Age: %@", face.attributes.age.stringValue];
                obj.genderText = [NSString stringWithFormat:@"Gender: %@", face.attributes.gender];
                obj.headPoseText = [NSString stringWithFormat:@"Head Pose(in degrees): roll(%@), yaw(%@)", face.attributes.headPose.roll.stringValue, face.attributes.headPose.yaw.stringValue];
                obj.moustacheText = [NSString stringWithFormat:@"Moustache: %@, Beard %@", face.attributes.facialHair.mustache.stringValue, face.attributes.facialHair.beard.stringValue];
                obj.smileText = [NSString stringWithFormat:@"Smile: %@", face.attributes.smile];

                [self.faceCellObjects addObject:obj];
                
            }
            [self.collectionView reloadData];
           
        }
        
        //hide loading indicator
        [self dismissViewControllerAnimated:YES completion:nil];
    }];
}
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    
    UIImage *chosenImage = info[UIImagePickerControllerEditedImage];
    self.imageView.image = chosenImage;
    [picker dismissViewControllerAnimated:YES completion:nil];
}

- (IBAction)detectionButtonPressed:(id)sender {
    
    if (self.imageView.image) {
        [self runDetection];
    }
}

- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section
{
    return [self.faceCellObjects count];
}
- (UICollectionViewCell *)collectionView:(UICollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    MPODetectionCollectionViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:@"cell" forIndexPath:indexPath];
    
    MPODetectionFaceCellObject *obj = [self.faceCellObjects objectAtIndex:indexPath.row];
    
    cell.ageLabel.text = obj.ageText;
    cell.genderLabel.text = obj.genderText;
    cell.headPoseLabel.text = obj.headPoseText;
    cell.moustacheLabel.text = obj.moustacheText;
    cell.smileLabel.text = obj.smileText;
    cell.imageView.image = obj.croppedFaceImage;
    
    return cell;
}
- (CGSize)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout*)collectionViewLayout sizeForItemAtIndexPath:(NSIndexPath *)indexPath {
    return CGSizeMake(self.collectionView.frame.size.width, 140);
}
@end
