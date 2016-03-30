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
using System.Windows;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namesapce for VisionServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VisionAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for AnalyzeInDomainPage.xaml
    /// </summary>
    public partial class AnalyzeInDomainPage : ImageScenarioPage
    {
        public AnalyzeInDomainPage()
        {
            InitializeComponent();
            this.PreviewImage = _imagePreview;
            this.URLTextBox = _urlTextBox;
        }

        /// <summary>
        /// Get a list of available domain models
        /// </summary>
        /// <returns></returns>
        private async Task<ModelResult> GetAvailableDomainModels()
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
            // Analyze the url against the given domain
            //
            Log("Calling VisionServiceClient.ListModelsAsync()...");
            ModelResult modelResult = await VisionServiceClient.ListModelsAsync();
            return modelResult;

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Uploads the image to Project Oxford and performs analysis against a given domain
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="domainModel">The domain model to analyze against</param>
        /// <returns></returns>
        private async Task<AnalysisInDomainResult> UploadAndAnalyzeInDomainImage(string imageFilePath, Model domainModel)
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
                // Analyze the image for the given domain
                //
                Log("Calling VisionServiceClient.AnalyzeImageInDomainAsync()...");
                AnalysisInDomainResult analysisResult = await VisionServiceClient.AnalyzeImageInDomainAsync(imageFileStream, domainModel);
                return analysisResult;
            }

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Sends a url to Project Oxford and performs analysis against a given domain
        /// </summary>
        /// <param name="imageUrl">The url of the image to analyze</param>
        /// <param name="domainModel">The domain model to analyze against</param>
        /// <returns></returns>
        private async Task<AnalysisInDomainResult> AnalyzeInDomainUrl(string imageUrl, Model domainModel)
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
            // Analyze the url against the given domain
            //
            Log("Calling VisionServiceClient.AnalyzeImageInDomainAsync()...");
            AnalysisInDomainResult analysisResult = await VisionServiceClient.AnalyzeImageInDomainAsync(imageUrl, domainModel);
            return analysisResult;

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        protected override async Task DoWork(Uri imageUri, bool upload)
        {
            _status.Text = "Analyzing...";

            Model domainModel = _domainModelComboBox.SelectedItem as Model;

            //
            // Either upload an image, or supply a url
            //
            AnalysisInDomainResult analysisInDomainResult;
            if (upload)
            {
                analysisInDomainResult = await UploadAndAnalyzeInDomainImage(imageUri.LocalPath, domainModel);
            }
            else
            {
                analysisInDomainResult = await AnalyzeInDomainUrl(imageUri.AbsoluteUri, domainModel);
            }
            _status.Text = "Analyzing Done";

            //
            // Log analysis result in the log window
            //
            Log("");
            Log("Analysis In Domain Result:");
            LogAnalysisInDomainResult(analysisInDomainResult);

        }

        private async void LoadModelsButton_Click(object sender, RoutedEventArgs e)
        {
            _status.Text = "Loading models...";
            
            //
            // Get the avaialable models
            //
            var modelResult = await GetAvailableDomainModels();
            _domainModelComboBox.ItemsSource = modelResult.Models;

            _status.Text = "Loaded models";

            _stepTwoPanel.Visibility = Visibility.Visible;

            Log("Models loaded");

        }
    }
}
