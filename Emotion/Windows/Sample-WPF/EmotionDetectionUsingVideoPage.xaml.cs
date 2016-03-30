using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namesapce for EmotionServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Common.Contract;
using System.Windows.Threading;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace EmotionAPI_WPF_Samples
{
    public partial class EmotionDetectionUsingVideoPage : Page
    {
        private static readonly TimeSpan QueryWaitTime = TimeSpan.FromSeconds(20);

        private EmotionListUserControl _emotionListControl;

        private DispatcherTimer _emotionSyncTimer;

        private VideoAggregateRecognitionResult _videoResult;

        public EmotionDetectionUsingVideoPage()
        {
            InitializeComponent();
            _emotionListControl = (EmotionListUserControl)this._videoResultControl.ResultControl;
        }

        /// <summary>
        /// Uploads the video to Project Oxford and detects emotions.
        /// </summary>
        /// <param name="videoFilePath">The video file path.</param>
        /// <returns></returns>
        private async Task<VideoAggregateRecognitionResult> UploadAndDetectEmotions(string videoFilePath)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            string subscriptionKey = window.ScenarioControl.SubscriptionKey;

            window.Log("EmotionServiceClient is created");

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Emotion API Service client
            //
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(subscriptionKey);

            window.Log("Calling EmotionServiceClient.RecognizeInVideoAsync()...");
            try
            {
                using (Stream videoFileStream = File.OpenRead(videoFilePath))
                {
                    //
                    // Upload the video, and tell the server to start recognizing emotions
                    //
                    window.Log("Start uploading video");
                    VideoEmotionRecognitionOperation videoOperation = await emotionServiceClient.RecognizeInVideoAsync(videoFileStream);
                    window.Log("Finished uploading video");


                    //
                    // Starts querying service status
                    //
                    VideoOperationResult result;
                    while (true)
                    {
                        result = await emotionServiceClient.GetOperationResultAsync(videoOperation);
                        if (result.Status == VideoOperationStatus.Succeeded || result.Status == VideoOperationStatus.Failed)
                        {
                            break;
                        }

                        window.Log(string.Format("Server status: {0}, wait {1} seconds...", result.Status, QueryWaitTime.TotalSeconds));
                        await Task.Delay(QueryWaitTime);
                    }

                    window.Log("Finish processing with server status: " + result.Status);

                    //
                    // Processing finished, checks result
                    // 
                    if (result.Status == VideoOperationStatus.Succeeded)
                    {
                        //
                        // Get the processing result by casting to the actual operation result
                        //
                        VideoAggregateRecognitionResult aggregateResult = ((VideoOperationInfoResult<VideoAggregateRecognitionResult>)result).ProcessingResult;
                        return aggregateResult;
                    }
                    else
                    {
                        // Failed
                        window.Log("Fail reason: " + result.Message);
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                window.Log(exception.ToString());
                return null;
            }
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------

        }

        private async void LoadVideoButton_Click(object sender, RoutedEventArgs e)
        {
            _loadVideoButton.IsEnabled = false;
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "Video files (*.mp4, *.mov, *.wmv)|*.mp4;*.mov;*.wmv";
            bool? result = openDlg.ShowDialog(window);

            if (!(bool)result)
            {
                return;
            }

            string videoFilePath = openDlg.FileName;
            Uri videoUri = new Uri(videoFilePath);

            _videoResultControl.IsWorking = true;
            var operationResult = await UploadAndDetectEmotions(videoFilePath);
            _videoResultControl.IsWorking = false;

            if (operationResult != null)
            {
                _videoResult = operationResult;
                _videoResultControl.SourceUri = videoUri;
                if (_emotionSyncTimer != null)
                {
                    _emotionSyncTimer.Stop();
                }

                _emotionSyncTimer = new DispatcherTimer();
                _emotionSyncTimer.Interval = TimeSpan.FromMilliseconds(500);
                _emotionSyncTimer.Tick += (e2, s) => { UpdateEmotionForTime(); };
                _emotionSyncTimer.Start();

                LogVideoFragments();
            }

            _loadVideoButton.IsEnabled = true;
        }

        private void LogVideoFragments()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            window.Log("Emotion recognition results:");
            window.Log(Newtonsoft.Json.JsonConvert.SerializeObject(_videoResult));
        }

        private void UpdateEmotionForTime()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            // get the current position of the video in terms of the timescale
            var currentTimeInTimescale = _videoResultControl.CurrentTime.TotalSeconds * _videoResult.Timescale;

            // the emotion results are aggregates of a two second window
            // get the emotion for one second in the future, so that the emotion displayed is the aggregate for the actual part of the video we're showing
            currentTimeInTimescale += _videoResult.Timescale;

            // find the fragment that represents where we are in the video
            var currentFragment = _videoResult.Fragments.SingleOrDefault((f) => f.Start < currentTimeInTimescale && f.Start + f.Duration > currentTimeInTimescale);
            if (currentFragment != null && currentFragment.Events != null)
            {
                // work out which block of events represents the current time
                int timeIndex = (int)Math.Floor((double)((currentTimeInTimescale - currentFragment.Start) / currentFragment.Interval.Value));

                // find the closest block of events that actually contains data
                var closestEventIndex = currentFragment.Events.Select((e, i) => i).Where((i) => currentFragment.Events[i].Length > 0).OrderBy((i) => Math.Abs(i - timeIndex)).FirstOrDefault();

                // get the events that occured at this time
                var events = currentFragment.Events[closestEventIndex];

                // update the UI, if there were new events
                if(events.Length > 0)
                {
                    // only show the first event per time period in this demo
                    window.ListVideoEmotionResult(_emotionListControl._resultListBox, events[0]);
                }
            }
        }
    }
}
