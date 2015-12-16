// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Project Oxford: http://ProjectOxford.ai
// 
// Project Oxford SDK Github:
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
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Microsoft.Win32;
using Microsoft.ProjectOxford.Speech.SpeakerIdentification;

namespace SPIDIdentificationAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for IdentifyFilePage.xaml
    /// </summary>
    public partial class IdentifyFilePage : Page
    {
        private string _selectedFile = "";

        private SpeechIdServiceClient _serviceClient;

        /// <summary>
        /// Constructor to initialize the Identify File page
        /// </summary>
        public IdentifyFilePage()
        {
            InitializeComponent();

            _speakersListFrame.Navigate(SpeakersListPage.SpeakersList);

            MainWindow window = (MainWindow)Application.Current.MainWindow;
            _serviceClient = new SpeechIdServiceClient(window.ScenarioControl.SubscriptionKey);
        }

        private void _loadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "WAV Files(*.wav)|*.wav";
            bool? result = openFileDialog.ShowDialog(window);

            if (!(bool)result)
            {
                window.Log("No File Selected.");
                return;
            }
            window.Log("File Selected: " + openFileDialog.FileName);
            _selectedFile = openFileDialog.FileName;
        }

        private async void _identifyBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                if (_selectedFile == "")
                    throw new Exception("No File Selected.");

                window.Log("Identifying File...");
                IdentificationProfile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();
                string[] testProfileIds = new string[selectedProfiles.Length];
                for (int i = 0; i < testProfileIds.Length; i++)
                {
                    testProfileIds[i] = selectedProfiles[i].IdentificationProfileId;
                }

                IdentificationResponse response;
                using (Stream audioStream = File.OpenRead(_selectedFile))
                {
                    _selectedFile = "";
                    response = await _serviceClient.IdentiftyAsync(audioStream, testProfileIds, TimeSpan.FromSeconds(5), 10);
                }
                window.Log("Identification Done.");

                _identificationResultTxtBlk.Text = response.IdentifiedProfileId;
                switch (response.Confidence)
                {
                    case IdentificationConfidence.High:
                        _identificationConfidenceTxtBlk.Text = "High";
                        break;
                    case IdentificationConfidence.Low:
                        _identificationConfidenceTxtBlk.Text = "Low";
                        break;
                    case IdentificationConfidence.None:
                        _identificationConfidenceTxtBlk.Text = "Can't Identify Audio";
                        break;
                    case IdentificationConfidence.Normal:
                        _identificationConfidenceTxtBlk.Text = "Normal";
                        break;
                }
                _identificationResultStckPnl.Visibility = Visibility.Visible;
            }
            catch (IdentificationException ex)
            {
                window.Log("Speaker Identification Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                window.Log("Error: " + ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SpeakersListPage.SpeakersList.SetMultipleSelectionMode();
        }
    }
}
