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

#import "MPOVerificationViewController.h"
#import <ProjectOxfordFace/MPOFaceServiceClient.h>
#import "MPOUtilities.h"
#import "MPOImageCollectionViewCell.h"
#import "MPODemoConstants.h"
#import "MPOActivityIndicatorViewController.h"
@interface MPOVerificationFaceCellObject : NSObject
@property (nonatomic, strong) MPOFace *face;
@property (nonatomic, strong) UIImage *croppedFaceImage;
@end

@implementation MPOVerificationFaceCellObject
@end

@interface MPOVerificationViewController ()
@property (nonatomic, assign) NSInteger imageViewIndexSelected;
@property (nonatomic, strong) NSMutableArray *firstFaceCellObjects;
@property (nonatomic, strong) NSMutableArray *secondFaceCellObjects;
@property (nonatomic, assign) NSInteger indexOfFirstSelectedFace;
@property (nonatomic, assign) NSInteger indexOfSecondSelectedFace;
@end

@implementation MPOVerificationViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    
    self.firstFaceCellObjects = [[NSMutableArray alloc] init];
    self.secondFaceCellObjects = [[NSMutableArray alloc] init];
    
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


- (IBAction)verifyButtonPressed:(id)sender {
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:ProjectOxfordFaceSubscriptionKey];
    
    if ([self.firstFaceCellObjects count] != 0 && [self.secondFaceCellObjects count] != 0 ) {
        
        //show loading indicator
        MPOActivityIndicatorViewController *indicatorViewController = [[MPOActivityIndicatorViewController alloc] init];
        [self presentViewController:indicatorViewController animated:YES completion:nil];
        
        MPOVerificationFaceCellObject *firstSelectedFaceCellObject = self.firstFaceCellObjects[self.indexOfFirstSelectedFace];
        MPOVerificationFaceCellObject *secondSelectedFaceCellObject = self.secondFaceCellObjects[self.indexOfSecondSelectedFace];
        
        [client verifyWithFirstFaceId:firstSelectedFaceCellObject.face.faceId faceId2:secondSelectedFaceCellObject.face.faceId completionBlock:^(MPOVerifyResult *verifyResult, NSError *error) {
            
            //hide loading indicator
            [self dismissViewControllerAnimated:YES completion:nil];
            
            if (error) {
                NSLog(@"Error: %@", error);
            }
            else {
                NSString *message;
                
                if (verifyResult.isIdentical) {
                    message = [NSString stringWithFormat:@"The person is the same. The confidence is %@", verifyResult.confidence];
                }
                else {
                    message = [NSString stringWithFormat:@"The person is not the same. The confidence is %@", verifyResult.confidence];
                }
                
                UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"Verification Result"
                                                                                         message:message
                                                                                  preferredStyle:UIAlertControllerStyleAlert];
                
                UIAlertAction *okAction = [UIAlertAction actionWithTitle:@"OK"
                                                                   style:UIAlertActionStyleDefault
                                                                 handler:nil];
                [alertController addAction:okAction];
                [self presentViewController:alertController animated:YES completion:nil];
                
            }
            
        }];
    }
}

    
- (IBAction)firstFaceSelectImageButtonPressed:(id)sender {
    self.imageViewIndexSelected = 0;
    [self displaySelectionImageActionSheet];
    [self.firstFaceCellObjects removeAllObjects];
}

- (IBAction)secondFaceSelectImageButtonPressed:(id)sender {
    self.imageViewIndexSelected = 1;
    [self displaySelectionImageActionSheet];
    [self.secondFaceCellObjects removeAllObjects];
}

- (void)displaySelectionImageActionSheet {
    
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

- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    
    UIImage *chosenImage = info[UIImagePickerControllerEditedImage];
    
    if (self.imageViewIndexSelected == 0) {
        self.firstFaceImageView.image = chosenImage;
    }
    else if (self.imageViewIndexSelected == 1) {
        self.secondFaceImageView.image = chosenImage;
    }
    
    [picker dismissViewControllerAnimated:YES completion:nil];
    
    [self runDetection];
}

- (void)runDetection {
    
    //show loading indicator
    MPOActivityIndicatorViewController *indicatorViewController = [[MPOActivityIndicatorViewController alloc] init];
    [self presentViewController:indicatorViewController animated:YES completion:nil];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:ProjectOxfordFaceSubscriptionKey];
    
    UIImage *currentImage;
    
    if (self.imageViewIndexSelected == 0) {
        currentImage = self.firstFaceImageView.image;
    }
    else if (self.imageViewIndexSelected == 1) {
        currentImage = self.secondFaceImageView.image;
    }
    
    NSData *data = UIImageJPEGRepresentation(currentImage, 0.8);
    [client detectWithData:data returnFaceId:YES returnFaceLandmarks:YES returnFaceAttributes:@[] completionBlock:^(NSArray<MPOFace *> *collection, NSError *error) {
        
        if (error) {
            NSLog(@"Error: %@", error);
        }
        else {
            
            for (MPOFace *face in collection) {
                UIImage *croppedImage = [MPOUtilities cropImageToFaceRectangle:currentImage faceRectangle:face.faceRectangle];
                MPOVerificationFaceCellObject *obj = [[MPOVerificationFaceCellObject alloc] init];
                obj.croppedFaceImage = croppedImage;
                obj.face = face;
                
                if (self.imageViewIndexSelected == 0) {
                    [self.firstFaceCellObjects addObject:obj];
                }
                else if (self.imageViewIndexSelected == 1) {
                    [self.secondFaceCellObjects addObject:obj];
                }
            }
            
            if (self.imageViewIndexSelected == 0) {
                [self.firstFaceCollectionView reloadData];
            }
            else if (self.imageViewIndexSelected == 1) {
                [self.secondFaceCollectionView reloadData];
            }
            
        }
        
        //hide loading indicator
        [self dismissViewControllerAnimated:YES completion:nil];
    }];
}

- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section
{
    if (collectionView == self.firstFaceCollectionView) {
        return [self.firstFaceCellObjects count];
    }
    else {
        return [self.secondFaceCellObjects count];
    }
}
- (UICollectionViewCell *)collectionView:(UICollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    MPOImageCollectionViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:@"cell" forIndexPath:indexPath];
   
    MPOVerificationFaceCellObject *obj;
    NSInteger selectedIndex;
    
    if (collectionView == self.firstFaceCollectionView) {
        obj = [self.firstFaceCellObjects objectAtIndex:indexPath.row];
        selectedIndex = self.indexOfFirstSelectedFace;
    }
    else {
        obj = [self.secondFaceCellObjects objectAtIndex:indexPath.row];
        selectedIndex = self.indexOfSecondSelectedFace;
    }
    
    if (indexPath.row == selectedIndex) {
        cell.imageView.layer.borderWidth = 2;
        cell.imageView.layer.borderColor = [UIColor redColor].CGColor;
    }
    else {
        cell.imageView.layer.borderWidth = 0;
    }
    
    cell.imageView.image = obj.croppedFaceImage;
    
    return cell;
}
- (CGSize)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout*)collectionViewLayout sizeForItemAtIndexPath:(NSIndexPath *)indexPath {
    return CGSizeMake(self.firstFaceCollectionView.frame.size.width, 80);
}

- (void)collectionView:(UICollectionView *)collectionView didSelectItemAtIndexPath:(NSIndexPath *)indexPath
{
    if (collectionView == self.firstFaceCollectionView) {
        self.indexOfFirstSelectedFace = indexPath.row;
        [self.firstFaceCollectionView reloadData];
    }
    else {
        self.indexOfSecondSelectedFace = indexPath.row;
        [self.secondFaceCollectionView reloadData];
    }
}
@end
