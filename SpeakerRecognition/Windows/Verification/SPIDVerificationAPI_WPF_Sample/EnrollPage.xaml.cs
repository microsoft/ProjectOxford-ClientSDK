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

using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.ProjectOxford.Speech.SpeakerVerification;

namespace SPIDVerificationAPI_WPF_Sample
{
    /// <summary>
    /// Interaction logic for EnrollPage.xaml
    /// </summary>
    public partial class EnrollPage : Page
    {
        private string _subscriptionKey;
        private string _speakerId = null;
        private int _remainingEnrollments;
        private WaveIn _waveIn;
        private WaveFileWriter _fileWriter;
        private Stream _stream;
        private SpeechVerificationServiceClient _serviceClient;

        /// <summary>
        /// Creates a new EnrollPage 
        /// </summary>
        /// <param name="subscriptionKey">The subscription key</param>
        public EnrollPage()
        {
            InitializeComponent();
            _subscriptionKey = ((MainWindow)Application.Current.MainWindow).SubscriptionKey;
            _serviceClient = new SpeechVerificationServiceClient(_subscriptionKey);
            initializeRecorder();
            initializeSpeaker();
        }

        /// <summary>
        /// Initialize the speaker information
        /// </summary>
        private async void initializeSpeaker()
        {
            IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
            _speakerId = _storageHelper.readValue(MainWindow.SPEAKER_FILENAME);
            record.IsEnabled = false;
            if (_speakerId == null)
            {
                bool created = await createProfile();
                if (created)
                {
                    refreshPhrases();
                }
            }
            else
            {
                setStatus("Using profile Id: " + _speakerId);
                refreshPhrases();
                string enrollmentsStatus = _storageHelper.readValue(MainWindow.SPEAKER_ENROLLMENTS);
                if ((enrollmentsStatus != null) && (enrollmentsStatus.Equals("Done")))
                {
                    resetBtn.IsEnabled = true;
                }
            }

        }

        /// <summary>
        /// Initialize NAudio recorder instance
        /// </summary>
        private void initializeRecorder()
        {
            _waveIn = new WaveIn();
            _waveIn.DeviceNumber = 0;
            int sampleRate = 16000; // 16 kHz
            int channels = 1; // mono
            _waveIn.WaveFormat = new WaveFormat(sampleRate, channels);
            _waveIn.DataAvailable += waveIn_DataAvailable;
            _waveIn.RecordingStopped += waveSource_RecordingStopped;
        }

        /// <summary>
        /// A listener called when the recording stops
        /// </summary>
        /// <param name="sender">Sender object responsible for event</param>
        /// <param name="e">A set of arguments sent to the listener</param>
        private void waveSource_RecordingStopped(object sender, StoppedEventArgs e)
        {
            _fileWriter.Dispose();
            _fileWriter = null;
            _stream.Seek(0, SeekOrigin.Begin);
            //Dispose recorder object
            _waveIn.Dispose();
            initializeRecorder();
            enrollSpeaker(_stream);
        }

        /// <summary>
        /// A method that's called whenever there's a chunk of audio is recorded
        /// </summary>
        /// <param name="sender">The sender object responsible for the event</param>
        /// <param name="e">The arguments of the event object</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void waveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (_fileWriter == null)
            {
                _stream = new IgnoreDisposeStream(new MemoryStream());
                _fileWriter = new WaveFileWriter(_stream, _waveIn.WaveFormat);
            }
            _fileWriter.WriteData(e.Buffer, 0, e.BytesRecorded);
        }

        /// <summary>
        /// Enrolls the audio of the speaker
        /// </summary>
        /// <param name="audioStream">The audio stream</param>
        private async void enrollSpeaker(Stream audioStream)
        {
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                EnrollmentResponse response = await _serviceClient.EnrollStreamAsync(audioStream, _speakerId);
                sw.Stop();
                _remainingEnrollments = response.RemainingEnrollments;
                setStatus("Enrollment Done, Elapsed Time: " + sw.Elapsed);
                verPhraseText.Text = response.Phrase;
                setStatus("Your phrase: " + response.Phrase);
                setUserPhrase(response.Phrase);
                remEnrollText.Text = response.RemainingEnrollments.ToString();
                if (response.RemainingEnrollments == 0)
                {
                    MessageBox.Show("You have now completed the minimum number of enrollments. You may perform verification or add more enrollments", "Speaker enrolled");
                }
                resetBtn.IsEnabled = true;
                IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
                _storageHelper.writeValue(MainWindow.SPEAKER_ENROLLMENTS, "Done");
            }
            catch (EnrollmentException exception)
            {
                setStatus("Cannot enroll speaker: " + exception.Message);
            }
            catch (Exception gexp)
            {
                setStatus("Error: " + gexp.Message);
            }
        }

        /// <summary>
        /// Helper method to set the status bar message
        /// </summary>
        /// <param name="status">Status bar message</param>
        private void setStatus(string status)
        {
            ((MainWindow)Application.Current.MainWindow).Log(status);
        }

        /// <summary>
        /// Click handler for the record button
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">Event arguments object</param>
        private void record_Click(object sender, RoutedEventArgs e)
        {
            record.IsEnabled = false;
            stopRecord.IsEnabled = true;
            _waveIn.StartRecording();
            setStatus("Recording...");
        }

        /// <summary>
        /// Click handler for the record button
        /// </summary>
        /// <param name="sender">The object that sent the event</param>
        /// <param name="e">Event arguments object</param>
        private void stopRecord_Click(object sender, RoutedEventArgs e)
        {
            record.IsEnabled = true;
            stopRecord.IsEnabled = false;
            _waveIn.StopRecording();
            setStatus("Enrolling...");
        }

        /// <summary>
        /// Persists the verification phrase of the speaker
        /// </summary>
        /// <param name="phrase">The user phrase</param>
        private void setUserPhrase(string phrase)
        {
            IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
            _storageHelper.writeValue(MainWindow.SPEAKER_PHRASE_FILENAME, phrase);
        }

        /// <summary>
        /// Creates a speaker profile
        /// </summary>
        /// <returns>A boolean indicating the success/failure of the process</returns>
        private async Task<bool> createProfile()
        {
            setStatus("Creating Profile...");
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                SpeakerProfile response = await _serviceClient.CreateProfileAsync("en-us");
                sw.Stop();
                setStatus("Profile Created, Elapsed Time: " + sw.Elapsed);
                IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
                _storageHelper.writeValue(MainWindow.SPEAKER_FILENAME, response.VerificationProfileId);
                _speakerId = response.VerificationProfileId;
                return true;
            }
            catch (ProfileCreationException exception)
            {
                setStatus("Cannot create profile: " + exception.Message);
                return false;
            }
            catch (Exception gexp)
            {
                setStatus("Error: " + gexp.Message);
                return false;
            }
        }

        /// <summary>
        /// Refresh the list of phrases
        /// </summary>
        private async void refreshPhrases()
        {
            setStatus("Retrieving available phrases...");
            record.IsEnabled = false;
            try
            {
                List<PhraseResponse> phrases = await _serviceClient.GetAllAvailablePhrases("en-us");
                foreach (PhraseResponse phrase in phrases)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = phrase.Phrase;
                    phrasesList.Items.Add(item);
                }
                setStatus("Retrieving available phrases done");
            }
            catch (PhrasesException exp)
            {
                setStatus("Cannot retrieve phrases: " + exp.Message);
            }
            catch (Exception e)
            {
                setStatus("Error: " + e.Message);
            }
            record.IsEnabled = true;
        }

        /// <summary>
        /// Remove current speaker Id
        /// </summary>
        /// <param name="sender">Sender object responsible for event</param>
        /// <param name="e">A set of arguments sent to the listener</param>
        private async void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                setStatus("Resetting profile: " + _speakerId);
                await _serviceClient.ResetProfileAsync(_speakerId);
                setStatus("Profile reset");
                IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
                _storageHelper.writeValue(MainWindow.SPEAKER_ENROLLMENTS, "Empty");
                resetBtn.IsEnabled = false;
                remEnrollText.Text = "";
                verPhraseText.Text = "";
            }
            catch (ResetProfileException exp)
            {
                setStatus("Cannot reset Profile: " + exp.Message);
            }
            catch (Exception gexp)
            {
                setStatus("Error: " + gexp.Message);
            }
        }
    }
}
