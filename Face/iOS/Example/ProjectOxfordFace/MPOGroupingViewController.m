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


#import "MPOGroupingViewController.h"
#import "MPOUtilities.h"
#import <ProjectOxfordFace/MPOFaceServiceClient.h>
#import "MPOImageCollectionViewCell.h"
#import "MPOGroupingTableViewCell.h"
#import "MPOGroupingFaceCellObject.h"
#import "MPODemoConstants.h"
#import "MPOActivityIndicatorViewController.h"
@interface MPOGroupingViewController ()
@property (nonatomic, strong) NSMutableArray *faceCellObjects;
@property (nonatomic, strong) UIImage *lastSelectedImage;
@property (nonatomic, strong) NSMutableArray *groupingResults;
@property (nonatomic, assign) BOOL messyGroupExists;
@end


@implementation MPOGroupingViewController
- (void)viewDidLoad {
    [super viewDidLoad];
    self.facesCollectionView.dataSource = self;
    self.tableView.dataSource = self;
    
    self.faceCellObjects = [[NSMutableArray alloc] init];
    self.groupingResults = [[NSMutableArray alloc] init];

    self.numberOfFacesDetectedLabel.text = @"0 faces detected";
}

- (IBAction)addFacesButtonPressed:(id)sender {
    [self displaySelectionImageActionSheet];
}

- (IBAction)groupButtonPressed:(id)sender {
    //show loading indicator
    MPOActivityIndicatorViewController *indicatorViewController = [[MPOActivityIndicatorViewController alloc] init];
    [self presentViewController:indicatorViewController animated:YES completion:nil];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:ProjectOxfordFaceSubscriptionKey];
    
    NSMutableArray *faceIds = [[NSMutableArray alloc] init];
    
    for (MPOGroupingFaceCellObject *obj in self.faceCellObjects) {
        [faceIds addObject:obj.face.faceId];
    }
    
    NSMutableArray *allGroups = [[NSMutableArray alloc] init];

    [client groupWithFaceIds:faceIds completionBlock:^(MPOGroupResult *groupResult, NSError *error) {
        
        //add all of the normal group members if they exist
        for (NSArray *group in groupResult.groups) {
            
            NSMutableArray *currentGroup = [[NSMutableArray alloc] init];
            
            for (NSString *faceId in group) {
                [currentGroup addObject:[self cellForFaceId:faceId]];
            }
            
            [allGroups addObject:currentGroup];
        }
        
        //add all of the messey group members if they exist
        if (groupResult.messeyGroup.count != 0) {
            NSMutableArray *allGroupsMessyGroup = [[NSMutableArray alloc] init];
            
            for (NSString *faceId in groupResult.messeyGroup) {
                
                [allGroupsMessyGroup addObject:[self cellForFaceId:faceId]];
            }
            
            [allGroups addObject:allGroupsMessyGroup];
            
            self.messyGroupExists = TRUE;
        }
        else {
            self.messyGroupExists = FALSE;
        }
        
        self.groupingResults = allGroups;
        [self.tableView reloadData];
        
        //hide loading indicator
        [self dismissViewControllerAnimated:YES completion:nil];
    }];
}

- (MPOGroupingFaceCellObject *)cellForFaceId:(NSString *)faceId {
    
    MPOGroupingFaceCellObject *target = nil;
    
    for (MPOGroupingFaceCellObject *obj in self.faceCellObjects) {
        
        if ([obj.face.faceId isEqualToString:faceId]) {
            target = obj;
        }
    }
    
    return target;
}

- (void)runDetection {
    
    //show loading indicator
    MPOActivityIndicatorViewController *indicatorViewController = [[MPOActivityIndicatorViewController alloc] init];
    [self presentViewController:indicatorViewController animated:YES completion:nil];
    
    MPOFaceServiceClient *client = [[MPOFaceServiceClient alloc] initWithSubscriptionKey:ProjectOxfordFaceSubscriptionKey];
    
    NSData *data = UIImageJPEGRepresentation(self.lastSelectedImage, 0.8);
    [client detectWithData:data returnFaceId:YES returnFaceLandmarks:YES returnFaceAttributes:@[] completionBlock:^(NSArray<MPOFace *> *collection, NSError *error) {
        
        if (error) {
            NSLog(@"Error: %@", error);
        }
        else {
           
            for (MPOFace *face in collection) {
                UIImage *croppedImage = [MPOUtilities cropImageToFaceRectangle:self.lastSelectedImage faceRectangle:face.faceRectangle];
                
                MPOGroupingFaceCellObject *obj = [[MPOGroupingFaceCellObject alloc] init];
                obj.croppedFaceImage = croppedImage;
                obj.face = face;
                [self.faceCellObjects addObject:obj];
                
            }
            
             self.numberOfFacesDetectedLabel.text = [NSString stringWithFormat:@"%@ faces detected", @(self.faceCellObjects.count)];
            
            [self.facesCollectionView reloadData];

            //hide loading indicator
            [self dismissViewControllerAnimated:YES completion:nil];
        }
    }];
}
- (void)imagePickerController:(UIImagePickerController *)picker didFinishPickingMediaWithInfo:(NSDictionary *)info {
    
    UIImage *chosenImage = info[UIImagePickerControllerEditedImage];
    self.lastSelectedImage = chosenImage;
    [picker dismissViewControllerAnimated:YES completion:nil];
    
    if (chosenImage != nil) {
        [self runDetection];
    }
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

- (NSInteger)collectionView:(UICollectionView *)collectionView numberOfItemsInSection:(NSInteger)section
{
    return [self.faceCellObjects count];
}
- (UICollectionViewCell *)collectionView:(UICollectionView *)collectionView cellForItemAtIndexPath:(NSIndexPath *)indexPath
{
    MPOImageCollectionViewCell *cell = [collectionView dequeueReusableCellWithReuseIdentifier:@"cell" forIndexPath:indexPath];
    
    MPOGroupingFaceCellObject *obj = [self.faceCellObjects objectAtIndex:indexPath.row];
    
    cell.imageView.image = obj.croppedFaceImage;
    
    return cell;
}
- (CGSize)collectionView:(UICollectionView *)collectionView layout:(UICollectionViewLayout*)collectionViewLayout sizeForItemAtIndexPath:(NSIndexPath *)indexPath {
    return CGSizeMake(70, 70);
}

- (NSInteger)numberOfSectionsInTableView:(UITableView *)theTableView
{
    return [self.groupingResults count];
}

- (NSInteger)tableView:(UITableView *)theTableView numberOfRowsInSection:    (NSInteger)section
{
    return 1;
}

- (NSString *)tableView:(UITableView *)tableView titleForHeaderInSection:(NSInteger)section {
    if (self.messyGroupExists && section == self.groupingResults.count - 1) {
        return @"Messy Group";
    }
    else {
        return [NSString stringWithFormat:@"Group %@", @(section)];
    }
}
- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    MPOGroupingTableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:@"cell"];
    
    NSArray *currentGroup = [self.groupingResults objectAtIndex:indexPath.section];
    
    cell.group = currentGroup;
    [cell.collectionView reloadData];
    
    return cell;
}

@end
