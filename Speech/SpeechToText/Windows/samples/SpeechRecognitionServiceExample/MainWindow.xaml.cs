//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK Github:
// https://github.com/Microsoft/ProjectOxfordSDK-Windows
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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;

using Microsoft.ProjectOxford.SpeechRecognition;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading;

namespace MicrosoftProjectOxfordExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // You can also put the primary key in app.config, instead of using UI.
        // string _subscriptionKey = ConfigurationManager.AppSettings["primaryKey"];
        string _subscriptionKey;

        string _luisAppID = ConfigurationManager.AppSettings["luisAppID"];
        string _luisSubscriptionID = ConfigurationManager.AppSettings["luisSubscriptionID"];

        string _recoLanguage = "en-us";

        private const string ShortWaveFile = @"whatstheweatherlike.wav";
        private const string LongWaveFile = @"batman.wav";

        private DataRecognitionClient _dataClient;
        private MicrophoneRecognitionClient _micClient;

        public bool IsMicrophoneClientShortPhrase { get; set; }
        public bool IsMicrophoneClientDictation { get; set; }
        public bool IsMicrophoneClientWithIntent { get; set; }
        public bool IsDataClientShortPhrase { get; set; }
        public bool IsDataClientWithIntent { get; set;  }
        public bool IsDataClientDictation { get; set; }

        /// <summary>
        /// The MAIS reco response event
        /// </summary>
        private AutoResetEvent _FinalResponseEvent;

        /// <summary>
        /// Gets or sets subscription key
        /// </summary>
        public string SubscriptionKey
        {
            get
            {
                return _subscriptionKey;
            }

            set
            {
                _subscriptionKey = value;
                OnPropertyChanged<string>();
            }
        }

        private readonly string IsolatedStorageSubscriptionKeyFileName = "Subscription.txt";
        private readonly string DefaultSubscriptionKeyPromptMessage = "Paste your subscription key here to start";

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Intialize();
            _FinalResponseEvent = new AutoResetEvent(false);
        }

        /// <summary>
        //  Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e">An System.EventArgs that contains the event data.</param>
        protected override void OnClosed(EventArgs e)
        {
            if (null != _dataClient)
            {
                _dataClient.Dispose();
            }

            if (null != _micClient)
            {
                _micClient.Dispose();
            }

            _FinalResponseEvent.Dispose();

            base.OnClosed(e);
        }

        private void Intialize()
        {
            IsMicrophoneClientShortPhrase = true;
            IsMicrophoneClientWithIntent = false;
            IsMicrophoneClientDictation = false;
            IsDataClientShortPhrase = false;
            IsDataClientWithIntent = false;
            IsDataClientDictation = false;

            // Set the default choice for the group of checkbox.
            _micRadioButton.IsChecked = true;

            SubscriptionKey = GetSubscriptionKeyFromIsolatedStorage();
        }

        /// <summary>
        /// Handles the Click event of the _startButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            //_logText.Text = String.Empty;
            _startButton.IsEnabled = false;
            _radioGroup.IsEnabled = false;

            if (IsMicrophoneClientShortPhrase)
            {
                // Short phrase recongition using microphone
                LogRecognitionStart("microphone", _recoLanguage, SpeechRecognitionMode.ShortPhrase);

                if (_micClient == null)
                {
                    _micClient = CreateMicrophoneRecoClient(SpeechRecognitionMode.ShortPhrase, _recoLanguage, SubscriptionKey);
                } 
                _micClient.StartMicAndRecognition();
            }
            else if (IsMicrophoneClientDictation)
            {
                // Long dictation recongition using microphone
                LogRecognitionStart("microphone", _recoLanguage, SpeechRecognitionMode.LongDictation);

                if (_micClient == null)
                {
                    _micClient = CreateMicrophoneRecoClient(SpeechRecognitionMode.LongDictation, _recoLanguage, SubscriptionKey);
                }
                _micClient.StartMicAndRecognition();
            }
            else if (IsMicrophoneClientWithIntent)
            {
                if (_micClient == null)
                {
                    _micClient = CreateMicrophoneRecoClientWithIntent(_recoLanguage);
                }
                _micClient.StartMicAndRecognition();
            }
            else if (IsDataClientShortPhrase)
            {
                LogRecognitionStart("short wav file", _recoLanguage, SpeechRecognitionMode.ShortPhrase);

                if (_dataClient == null)
                {
                    _dataClient = CreateDataRecoClient(SpeechRecognitionMode.ShortPhrase, _recoLanguage);
                }
                SendAudioHelper(_dataClient, ShortWaveFile);
            }
            else if (IsDataClientDictation)
            {
                LogRecognitionStart("long wav file", _recoLanguage, SpeechRecognitionMode.LongDictation);

                if (_dataClient == null)
                {
                    _dataClient = CreateDataRecoClient(SpeechRecognitionMode.LongDictation, _recoLanguage);
                }
                SendAudioHelper(_dataClient, LongWaveFile);
            }
            else if (IsDataClientWithIntent)
            {
                if (_dataClient == null)
                {
                    _dataClient = CreateDataRecoClientWithIntent(_recoLanguage, ShortWaveFile);
                }
                SendAudioHelper(_dataClient, ShortWaveFile);
            }
        }

        private void LogRecognitionStart(string recoSource, string recoLanguage, SpeechRecognitionMode recoMode)
        {
            WriteLine("\n--- Start speech recognition using " + recoSource + " with " + recoMode + " mode in " + recoLanguage + " language ----\n\n");
        }

        MicrophoneRecognitionClient CreateMicrophoneRecoClient(SpeechRecognitionMode recoMode, string language, string subscriptionKey)
        {
            MicrophoneRecognitionClient micClient = SpeechRecognitionServiceFactory.CreateMicrophoneClient(
                recoMode,
                language,                                                                 
                subscriptionKey);

            // Event handlers for speech recognition results
            micClient.OnMicrophoneStatus += OnMicrophoneStatus;
            micClient.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            if (recoMode == SpeechRecognitionMode.ShortPhrase)
            {
                micClient.OnResponseReceived += OnMicShortPhraseResponseReceivedHandler;
            }
            else if (recoMode == SpeechRecognitionMode.LongDictation)
            {
                micClient.OnResponseReceived += OnMicDictationResponseReceivedHandler;
            }
            micClient.OnConversationError += OnConversationErrorHandler;            

            return micClient;
        }

        MicrophoneRecognitionClient CreateMicrophoneRecoClientWithIntent(string recoLanguage)
        {
            WriteLine("--- Start microphone dictation with Intent detection ----");

            MicrophoneRecognitionClientWithIntent intentMicClient =
                SpeechRecognitionServiceFactory.CreateMicrophoneClientWithIntent(recoLanguage,
                                                                                 SubscriptionKey,
                                                                                 _luisAppID,
                                                                                 _luisSubscriptionID);
            intentMicClient.OnIntent += OnIntentHandler;

            // Event handlers for speech recognition results
            intentMicClient.OnMicrophoneStatus += OnMicrophoneStatus;
            intentMicClient.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            intentMicClient.OnResponseReceived += OnMicShortPhraseResponseReceivedHandler;
            intentMicClient.OnConversationError += OnConversationErrorHandler;

            intentMicClient.StartMicAndRecognition();

            return intentMicClient;

        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.projectoxford.ai/doc/general/subscription-key-mgmt");
        }

        /// <summary>
        ///     Speech recognition with data (for example from a file or audio source).  
        ///     The data is broken up into buffers and each buffer is sent to the Speech Recognition Service.
        ///     No modification is done to the buffers, so the user can apply their
        ///     own Silence Detection if desired.
        /// </summary>
        DataRecognitionClient CreateDataRecoClient(SpeechRecognitionMode recoMode, string recoLanguage)
        {
            DataRecognitionClient dataClient = SpeechRecognitionServiceFactory.CreateDataClient(
                recoMode,
                recoLanguage,
                SubscriptionKey);

            // Event handlers for speech recognition results
            if (recoMode == SpeechRecognitionMode.ShortPhrase)
            {
                dataClient.OnResponseReceived += OnDataShortPhraseResponseReceivedHandler;
            }
            else
            {
                dataClient.OnResponseReceived += OnDataDictationResponseReceivedHandler;
            }
            dataClient.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            dataClient.OnConversationError += OnConversationErrorHandler;

            return dataClient;
        }

        DataRecognitionClientWithIntent CreateDataRecoClientWithIntent(string recoLanguage, string wavFileName)
        {
            DataRecognitionClientWithIntent intentDataClient =
                SpeechRecognitionServiceFactory.CreateDataClientWithIntent(recoLanguage,
                                                                           SubscriptionKey,
                                                                           _luisAppID,
                                                                           _luisSubscriptionID);
            // Event handlers for speech recognition results
            intentDataClient.OnResponseReceived += OnDataShortPhraseResponseReceivedHandler;
            intentDataClient.OnPartialResponseReceived += OnPartialResponseReceivedHandler;
            intentDataClient.OnConversationError += OnConversationErrorHandler;

            // Event handler for intent result
            intentDataClient.OnIntent += OnIntentHandler;

            return intentDataClient;
        }

        private void SendAudioHelper(DataRecognitionClient dataClient, string wavFileName)
        {
            using (FileStream fileStream = new FileStream(wavFileName, FileMode.Open, FileAccess.Read))
            {
                // Note for wave files, we can just send data from the file right to the server.
                // In the case you are not an audio file in wave format, and instead you have just
                // raw data (for example audio coming over bluetooth), then before sending up any 
                // audio data, you must first send up an SpeechAudioFormat descriptor to describe 
                // the layout and format of your raw audio data via DataRecognitionClient's sendAudioFormat() method.

                int bytesRead = 0;
                byte[] buffer = new byte[1024];

                try
                {
                    do
                    {
                        // Get more Audio data to send into byte buffer.
                        bytesRead = fileStream.Read(buffer, 0, buffer.Length);

                        // Send of audio data to service. 
                        dataClient.SendAudio(buffer, bytesRead);
                    } while (bytesRead > 0);
                }
                finally
                {
                    // We are done sending audio.  Final recognition results will arrive in OnResponseReceived event call.
                    dataClient.EndAudio();
                }
            }
        }

        /// <summary>
        ///     Called when a final response is received; 
        /// </summary>
        void OnMicShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                WriteLine("--- OnMicShortPhraseResponseReceivedHandler ---");

                _FinalResponseEvent.Set();

                // we got the final result, so it we can end the mic reco.  No need to do this
                // for dataReco, since we already called endAudio() on it as soon as we were done
                // sending all the data.
                _micClient.EndMicAndRecognition();

                // BUGBUG: Work around for the issue when cached _micClient cannot be re-used for recognition.
                _micClient.Dispose();
                _micClient = null;

                WriteResponseResult(e);

                _startButton.IsEnabled = true;
                _radioGroup.IsEnabled = true;
            }));
        }

        /// <summary>
        ///     Called when a final response is received; 
        /// </summary>
        void OnDataShortPhraseResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            Dispatcher.Invoke((Action)(() =>
            {
                WriteLine("--- OnDataShortPhraseResponseReceivedHandler ---");
                // we got the final result, so it we can end the mic reco.  No need to do this
                // for dataReco, since we already called endAudio() on it as soon as we were done
                // sending all the data.

                _FinalResponseEvent.Set();

                WriteResponseResult(e);

                _startButton.IsEnabled = true;
                _radioGroup.IsEnabled = true;
            }));
        }

        private void WriteResponseResult(SpeechResponseEventArgs e)
        {
            if (e.PhraseResponse.Results.Length == 0)
            {
                WriteLine("No phrase resonse is available.");
            }
            else
            {
                WriteLine("********* Final n-BEST Results *********");
                for (int i = 0; i < e.PhraseResponse.Results.Length; i++)
                {
                    WriteLine("[{0}] Confidence={1}, Text=\"{2}\"",
                                    i, e.PhraseResponse.Results[i].Confidence,
                                    e.PhraseResponse.Results[i].DisplayText);
                }
                WriteLine();
            }
        }

        /// <summary>
        ///     Called when a final response is received; 
        /// </summary>
        void OnMicDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            WriteLine("--- OnMicDictationResponseReceivedHandler ---");
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.EndOfDictation ||
                e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            { 
                Dispatcher.Invoke((Action)(() => 
                {
                    _FinalResponseEvent.Set();

                    // we got the final result, so it we can end the mic reco.  No need to do this
                    // for dataReco, since we already called endAudio() on it as soon as we were done
                    // sending all the data.
                    _micClient.EndMicAndRecognition();

                    // BUGBUG: Work around for the issue when cached _micClient cannot be re-used for recognition.
                    _micClient.Dispose();
                    _micClient = null;

                    _startButton.IsEnabled = true;
                    _radioGroup.IsEnabled = true;

                }));                
            }
            WriteResponseResult(e);
        }

        /// <summary>
        ///     Called when a final response is received; 
        /// </summary>
        void OnDataDictationResponseReceivedHandler(object sender, SpeechResponseEventArgs e)
        {
            WriteLine("--- OnDataDictationResponseReceivedHandler ---");
            if (e.PhraseResponse.RecognitionStatus == RecognitionStatus.EndOfDictation ||
                e.PhraseResponse.RecognitionStatus == RecognitionStatus.DictationEndSilenceTimeout)
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    _FinalResponseEvent.Set();

                    _startButton.IsEnabled = true;
                    _radioGroup.IsEnabled = true;

                    // we got the final result, so it we can end the mic reco.  No need to do this
                    // for dataReco, since we already called endAudio() on it as soon as we were done
                    // sending all the data.
                }));
            }
            WriteResponseResult(e);
        }


        /// <summary>
        ///     Called when a final response is received and its intent is parsed 
        /// </summary>
        void OnIntentHandler(object sender, SpeechIntentEventArgs e)
        {
            WriteLine("--- Intent received by OnIntentHandler() ---");
            WriteLine("{0}", e.Payload);
            WriteLine();
        }

        /// <summary>
        ///     Called when a partial response is received.
        /// </summary>
        void OnPartialResponseReceivedHandler(object sender, PartialSpeechResponseEventArgs e)
        {
            WriteLine("--- Partial result received by OnPartialResponseReceivedHandler() ---");
            WriteLine("{0}", e.PartialResult);
            WriteLine();
        }

        /// <summary>
        ///     Called when an error is received.
        /// </summary>
        void OnConversationErrorHandler(object sender, SpeechErrorEventArgs e)
        {
           Dispatcher.Invoke(() =>
           {
               _startButton.IsEnabled = true;
               _radioGroup.IsEnabled = true;
           });

            WriteLine("--- Error received by OnConversationErrorHandler() ---");
            WriteLine("Error code: {0}", e.SpeechErrorCode.ToString());
            WriteLine("Error text: {0}", e.SpeechErrorText);
            WriteLine();
        }

        /// <summary>
        ///     Called when the microphone status has changed.
        /// </summary>
        void OnMicrophoneStatus(object sender, MicrophoneEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                WriteLine("--- Microphone status change received by OnMicrophoneStatus() ---");
                WriteLine("********* Microphone status: {0} *********", e.Recording);
                if (e.Recording)
                {
                    WriteLine("Please start speaking.");
                }
                WriteLine();
            });
        }


        /// <summary>
        /// Writes the line.
        /// </summary>
        void WriteLine()
        {
            WriteLine(string.Empty);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        void WriteLine(string format, params object[] args)
        {
            var formattedStr = string.Format(format, args);
            Trace.WriteLine(formattedStr);
            Dispatcher.Invoke(() =>
            {
                _logText.Text += (formattedStr + "\n");
                _logText.ScrollToEnd();
            });
        }

        /// <summary>
        /// Gets the subscription key from isolated storage.
        /// </summary>
        /// <returns></returns>
        private string GetSubscriptionKeyFromIsolatedStorage()
        {
            string subscriptionKey = null;

            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                try
                {
                    using (var iStream = new IsolatedStorageFileStream(IsolatedStorageSubscriptionKeyFileName, FileMode.Open, isoStore))
                    {
                        using (var reader = new StreamReader(iStream))
                        {
                            subscriptionKey = reader.ReadLine();
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    subscriptionKey = null;
                }
            }
            if (string.IsNullOrEmpty(subscriptionKey))
            {
                subscriptionKey = DefaultSubscriptionKeyPromptMessage;
            }
            return subscriptionKey;
        }

        /// <summary>
        /// Saves the subscription key to isolated storage.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        private void SaveSubscriptionKeyToIsolatedStorage(string subscriptionKey)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                using (var oStream = new IsolatedStorageFileStream(IsolatedStorageSubscriptionKeyFileName, FileMode.Create, isoStore))
                {
                    using (var writer = new StreamWriter(oStream))
                    {
                        writer.WriteLine(subscriptionKey);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the subscription key save button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void SaveKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSubscriptionKeyToIsolatedStorage(SubscriptionKey);
                MessageBox.Show("Subscription key is saved in your disk.\nYou do not need to paste the key next time.", "Subscription Key");
            }
            catch (System.Exception exception)
            {
                MessageBox.Show("Fail to save subscription key. Error message: " + exception.Message,
                    "Subscription Key", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteKey_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SubscriptionKey = DefaultSubscriptionKeyPromptMessage;
                SaveSubscriptionKeyToIsolatedStorage("");
                MessageBox.Show("Subscription key is deleted from your disk.", "Subscription Key");
            }
            catch (System.Exception exception)
            {
                MessageBox.Show("Fail to delete subscription key. Error message: " + exception.Message,
                    "Subscription Key", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Helper function for INotifyPropertyChanged interface 
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="caller">Property name</param>
        private void OnPropertyChanged<T>([CallerMemberName]string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset everything
            if (_micClient != null)
            {
                _micClient.EndMicAndRecognition();
                _micClient.Dispose();
            }
            if (_dataClient != null)
            {
                _dataClient.Dispose();
            }
            _micClient = null;
            _dataClient = null;

            _logText.Text = "";
            _startButton.IsEnabled = true;
            _radioGroup.IsEnabled = true;
        }
    }
}
