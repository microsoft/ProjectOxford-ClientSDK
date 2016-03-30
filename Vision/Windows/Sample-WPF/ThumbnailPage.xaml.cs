// 
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
// 

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namesapce for VisionServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Vision;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VisionAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for ThumbnailPage.xaml
    /// </summary>
    public partial class ThumbnailPage : ImageScenarioPage
    {
        public ThumbnailPage()
        {
            InitializeComponent();
            this.PreviewImage = _imagePreview;
            this.URLTextBox = _urlTextBox;
        }

        /// <summary>
        /// Uploads the image to Project Oxford and generates a thumbnail
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="width">Width of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="height">Height of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="smartCropping">Boolean flag for enabling smart cropping.</param>
        /// <returns></returns>
        private async Task<byte[]> UploadAndThumbnailImage(string imageFilePath, int width, int height, bool smartCropping)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Vision API Service client
            //
            VisionServiceClient VisionServiceClient = new VisionServiceClient(SubscriptionKey);
            Log("VisionServiceClient is created");

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                //
                // Upload an image and generate a thumbnail
                //
                Log("Calling VisionServiceClient.GetThumbnailAsync()...");
                return await VisionServiceClient.GetThumbnailAsync(imageFileStream, width, height, smartCropping);
            }

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Sends a url to Project Oxford and generates a thumbnail
        /// </summary>
        /// <param name="imageUrl">The url of the image to generate a thumbnail for</param>
        /// <param name="width">Width of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="height">Height of the thumbnail. It must be between 1 and 1024. Recommended minimum of 50.</param>
        /// <param name="smartCropping">Boolean flag for enabling smart cropping.</param>
        /// <returns></returns>
        private async Task<byte[]> ThumbnailUrl(string imageUrl, int width, int height, bool smartCropping)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Vision API Service client
            //
            VisionServiceClient VisionServiceClient = new VisionServiceClient(SubscriptionKey);
            Log("VisionServiceClient is created");

            //
            // Generate a thumbnail for the given url
            //
            Log("Calling VisionServiceClient.GetThumbnailAsync()...");
            byte[] thumbnail = await VisionServiceClient.GetThumbnailAsync(imageUrl, width, height, smartCropping);
            return thumbnail;

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Perform the work for this scenario
        /// </summary>
        /// <param name="imageUri">The URI of the image to run against the scenario</param>
        /// <param name="upload">Upload the image to Project Oxford if [true]; submit the Uri as a remote url if [false];</param>
        /// <returns></returns>
        protected override async Task DoWork(Uri imageUri, bool upload)
        {
            _status.Text = "Generating Thumbnail...";

            //
            // Get the parameters
            //
            int width = int.Parse(_widthTextBox.Text);
            int height = int.Parse(_heightTextBox.Text);
            bool useSmartCropping = _smartCroppingCheckbox.IsChecked.Value;

            //
            // Either upload an image, or supply a url
            //
            byte[] thumbnail;
            if (upload)
            {
                thumbnail = await UploadAndThumbnailImage(imageUri.LocalPath, width, height, useSmartCropping);
            }
            else
            {
                thumbnail = await ThumbnailUrl(imageUri.AbsoluteUri, width, height, useSmartCropping);
            }
            _status.Text = "Thumbnail Generated";

            //
            // Show the generated thumbnail in the GUI
            //
            MemoryStream ms = new MemoryStream(thumbnail);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage thumbSource = new BitmapImage();
            thumbSource.BeginInit();
            thumbSource.CacheOption = BitmapCacheOption.None;
            thumbSource.StreamSource = ms;
            thumbSource.EndInit();

            _thumbPreview.Source = thumbSource;

            Log("Generated Thumbnail");
        }
    }
}
