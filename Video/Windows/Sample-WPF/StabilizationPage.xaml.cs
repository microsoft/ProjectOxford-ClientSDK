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
using System.Windows.Controls;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namespace for VideoServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Video.Contract;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VideoAPI_WPF_Samples
{
    public partial class StabilizationPage : Page
    {
        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);
        private const string LogIdentifier = "Video Stabilization";

        public StabilizationPage()
        {
            InitializeComponent();
            Resources.Add("_internalDataContext", _dataContext);
        }

        // -----------------------------------------------------------------------
        // KEY SAMPLE CODE STARTS HERE
        // -----------------------------------------------------------------------
        private async Task StabilizeVideo(string subscriptionKey, string originalFilePath)
        {
            _dataContext.IsWorking = true;
            _dataContext.SourceUri = null;
            _dataContext.ResultUri = null;

            Helpers.Log(LogIdentifier, "Start stabilizing");
            Microsoft.ProjectOxford.Video.VideoServiceClient client =
                new Microsoft.ProjectOxford.Video.VideoServiceClient(subscriptionKey);

            client.Timeout = TimeSpan.FromMinutes(10);

            using (FileStream originalStream = new FileStream(originalFilePath, FileMode.Open, FileAccess.Read))
            {
                // Creates a video operation of video stabilization
                Helpers.Log(LogIdentifier, "Start uploading video");
                Operation operation = await client.CreateOperationAsync(originalStream, new VideoStabilizationOperationSettings());
                Helpers.Log(LogIdentifier, "Uploading video done");

                // Starts querying service status
                OperationResult result = await client.GetOperationResultAsync(operation);
                while (result.Status != OperationStatus.Succeeded && result.Status != OperationStatus.Failed)
                {
                    Helpers.Log(LogIdentifier, "Server status: {0}, wait {1} seconds...", result.Status, QueryWaitTime.TotalSeconds);
                    await Task.Delay(QueryWaitTime);
                    result = await client.GetOperationResultAsync(operation);
                }
                Helpers.Log(LogIdentifier, "Finish processing with server status: " + result.Status);

                // Processing finished, checks result
                if (result.Status == OperationStatus.Succeeded)
                {
                    // Downloads generated video
                    string tmpFilePath = Path.GetTempFileName() + Path.GetExtension(originalFilePath);
                    Helpers.Log(LogIdentifier, "Start downloading result video");
                    using (Stream resultStream = await client.GetResultVideoAsync(result.ResourceLocation))
                    using (FileStream stream = new FileStream(tmpFilePath, FileMode.Create))
                    {
                        byte[] b = new byte[2048];
                        int length = 0;
                        while ((length = await resultStream.ReadAsync(b, 0, b.Length)) > 0)
                        {
                            await stream.WriteAsync(b, 0, length);
                        }
                    }
                    Helpers.Log(LogIdentifier, "Downloading result video done");

                    _dataContext.SourceUri = new Uri(originalFilePath);
                    _dataContext.ResultUri = new Uri(tmpFilePath);
                }
                else
                {
                    // Failed
                    Helpers.Log(LogIdentifier, "Fail reason: " + result.Message);
                }

                _dataContext.IsWorking = false;
            }
        }

        // -----------------------------------------------------------------------
        // KEY SAMPLE CODE ENDS HERE
        // -----------------------------------------------------------------------

        private async void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            string originalFilePath = Helpers.PickFile();
            if (originalFilePath == null) return;

            await StabilizeVideo(Helpers.SubscriptionKey, originalFilePath);
        }

        private readonly VideoResultDataContext _dataContext = new VideoResultDataContext();
    }
}
