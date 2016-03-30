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

using Microsoft.ProjectOxford.SpeakerRecognition;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract;
using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPIDIdentificationAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for EnrollSpeakersPage.xaml
    /// </summary>
    public partial class EnrollSpeakersPage : Page
    {
        private string _selectedFile = "";

        private SpeakerIdentificationServiceClient _serviceClient;

        /// <summary>
        /// Constructor to initialize the Enroll Speakers page
        /// </summary>
        public EnrollSpeakersPage()
        {
            InitializeComponent();

            _speakersListFrame.Navigate(SpeakersListPage.SpeakersList);

            MainWindow window = (MainWindow)Application.Current.MainWindow;
            _serviceClient = new SpeakerIdentificationServiceClient(window.ScenarioControl.SubscriptionKey);
        }

        private async void _addBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                window.Log("Creating Speaker Profile...");
                CreateProfileResponse creationResponse = await _serviceClient.CreateProfileAsync(_localeCmb.Text);
                window.Log("Speaker Profile Created.");
                window.Log("Retrieving The Created Profile...");
                Profile profile = await _serviceClient.GetProfileAsync(creationResponse.ProfileId);
                window.Log("Speaker Profile Retrieved.");
                SpeakersListPage.SpeakersList.AddSpeaker(profile);
            }
            catch (CreateProfileException ex)
            {
                window.Log("Profile Creation Error: " + ex.Message);
            }
            catch (GetProfileException ex)
            {
                window.Log("Error Retrieving The Profile: " + ex.Message);
            }
            catch (Exception ex)
            {
                window.Log("Error: " + ex.Message);
            }
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

        private async void _enrollBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                if (_selectedFile == "")
                    throw new Exception("No File Selected.");

                window.Log("Enrolling Speaker...");
                Profile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();

                OperationLocation processPollingLocation;
                using (Stream audioStream = File.OpenRead(_selectedFile))
                {
                    _selectedFile = "";
                    processPollingLocation = await _serviceClient.EnrollAsync(audioStream, selectedProfiles[0].ProfileId);
                }

                EnrollmentOperation enrollmentResult;
                int numOfRetries = 10;
                TimeSpan timeBetweenRetries = TimeSpan.FromSeconds(5.0);
                while(numOfRetries > 0)
                {
                    await Task.Delay(timeBetweenRetries);
                    enrollmentResult = await _serviceClient.CheckEnrollmentStatusAsync(processPollingLocation);

                    if (enrollmentResult.Status == Status.Succeeded)
                    {
                        break;
                    }
                    else if (enrollmentResult.Status == Status.Failed)
                    {
                        throw new EnrollmentException(enrollmentResult.Message);
                    }
                    numOfRetries--;
                }
                if(numOfRetries <= 0)
                {
                    throw new EnrollmentException("Enrollment operation timeout.");
                }
                window.Log("Enrollment Done.");
                await SpeakersListPage.SpeakersList.UpdateAllSpeakersAsync();
            }
            catch (EnrollmentException ex)
            {
                window.Log("Enrollment Error: " + ex.Message);
            }
            catch (Exception ex)
            {
                window.Log("Error: " + ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SpeakersListPage.SpeakersList.SetSingleSelectionMode();
        }
    }
}

