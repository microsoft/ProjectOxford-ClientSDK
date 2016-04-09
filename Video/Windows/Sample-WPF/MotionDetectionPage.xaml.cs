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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SampleUserControlLibrary;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namespace for VideoServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Video;
using Microsoft.ProjectOxford.Video.Contract;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VideoAPI_WPF_Samples
{
    public partial class MotionDetectionPage : Page
    {
        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);
        private const string LogIdentifier = "Motion Detection";

        public MotionDetectionPage()
        {
            InitializeComponent();
            Resources.Add("_internalDataContext", _dataContext);
        }

        // -----------------------------------------------------------------------
        // KEY SAMPLE CODE STARTS HERE
        // -----------------------------------------------------------------------
        private async Task DetectMotion(string subscriptionKey, string originalFilePath)
        {
            _dataContext.IsWorking = true;
            _dataContext.SourceUri = null;
            _dataContext.ResultText = null;

            Helpers.Log(LogIdentifier, "Start motion detection");
            VideoServiceClient client = new VideoServiceClient(subscriptionKey);

            client.Timeout = TimeSpan.FromMinutes(10);

            using (FileStream originalStream = new FileStream(originalFilePath, FileMode.Open, FileAccess.Read))
            {
                // Creates a video operation of motion detection
                Helpers.Log(LogIdentifier, "Start uploading video");
                Operation operation = await client.CreateOperationAsync(originalStream, new MotionDetectionOperationSettings());
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
                    // Gets output JSON
                    _dataContext.SourceUri = new Uri(originalFilePath);
                    _dataContext.ResultText = Helpers.FormatJson<MotionDetectionResult>(result.ProcessingResult);
                    _dataContext.FrameHighlights = GetHighlights(result.ProcessingResult).ToList();
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

        /// <summary>
        /// This method parses the JSON ouput, and converts to a sequence of time frames with highlight region.  A full video highlight is created if there is motion detected in the frame.
        /// </summary>
        /// <param name="json">JSON output of motion detection result.</param>
        /// <returns>Sequence of time frames with highlight regions.</returns>
        private static IEnumerable<FrameHighlight> GetHighlights(string json)
        {
            MotionDetectionResult motionDetectionResult = Helpers.FromJson<MotionDetectionResult>(json);

            double timescale = motionDetectionResult.Timescale;

            if (motionDetectionResult.Regions == null) yield break;

            List<int> regionIds = motionDetectionResult.Regions.Select(x => x.Id).ToList();

            Rect hasMotionRect = new Rect(new Point(0, 0), new Size(1, 1));  // Uses this full-frame rectangle to represent motion is detected in one frame
            Rect noMotionRect = new Rect(new Point(0, 0), new Size(0, 0));   // Uses this empty rectangle to represent motion is not detected

            foreach (Fragment<MotionEvent> fragment in motionDetectionResult.Fragments)
            {
                if (fragment.Events == null || fragment.Events.Length == 0)
                {
                    // If 'Events' is empty, there isn't any motion detected in this fragment
                    Rect[] rects = new Rect[regionIds.Count];
                    for (int i = 0; i < rects.Length; i++) rects[i] = noMotionRect;

                    yield return new FrameHighlight() { Time = fragment.Start / timescale, HighlightRects = rects.ToArray() };
                }
                else
                {
                    long interval = fragment.Interval.GetValueOrDefault();

                    for (int i = 0; i < fragment.Events.Length; i++)
                    {
                        double currentTime = (fragment.Start + interval*i)/timescale;

                        MotionEvent[] evts = fragment.Events[i];

                        Rect[] rects = regionIds.Select(id =>
                        {
                            MotionEvent evt = evts.FirstOrDefault(x => x.RegionId == id);
                            if (evt == null) return noMotionRect;
                            return hasMotionRect;
                        }).ToArray();

                        yield return new FrameHighlight() {Time = currentTime, HighlightRects = rects};
                    }
                }
            }
        }

        private async void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            string originalFilePath = Helpers.PickFile();
            if (originalFilePath == null) return;

            await DetectMotion(Helpers.SubscriptionKey, originalFilePath);
        }

        private readonly VideoResultDataContext _dataContext = new VideoResultDataContext();
    }
}
