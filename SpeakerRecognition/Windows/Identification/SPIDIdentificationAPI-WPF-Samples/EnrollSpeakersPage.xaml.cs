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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace SPIDIdentificationAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for EnrollSpeakersPage.xaml
    /// </summary>
    public partial class EnrollSpeakersPage : Page
    {
        private string _selectedFile = "";
        public EnrollSpeakersPage()
        {
            InitializeComponent();

            _speakersListFrame.Navigate(SpeakersListPage.SpeakersList);
        }

        private async void _addBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                IdentificationServiceHttpClientHelper requestHelper = new IdentificationServiceHttpClientHelper(window.ScenarioControl.SubscriptionKey);
                window.Log("Creating Speaker Profile...");
                CreateProfileResponse creationResponse = await requestHelper.CreateProfileAsync(_localeCmb.Text);
                window.Log("Speaker Profile Created.");
                window.Log("Retreiving The Created Profile...");
                IdentificationProfile profile = await requestHelper.GetProfileAsync(creationResponse.IdentificationProfileId);
                window.Log("Speaker Profile Retreived.");
                SpeakersListPage.SpeakersList.AddSpeaker(profile);
            }
            catch (Exception ex)
            {
                window.Log(ex.Message);
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
                IdentificationServiceHttpClientHelper requestHelper = new IdentificationServiceHttpClientHelper(window.ScenarioControl.SubscriptionKey);
                if (_selectedFile == "")
                    throw new Exception("Error: No File Selected.");

                window.Log("Enrolling Speaker...");
                IdentificationProfile[] selectedProfiles = SpeakersListPage.SpeakersList.GetSelectedProfiles();
                using (Stream audioStream = File.OpenRead(_selectedFile))
                {
                    _selectedFile = "";
                    await requestHelper.EnrollAsync(audioStream, selectedProfiles[0].IdentificationProfileId);
                }
                window.Log("Enrollment Done.");
                await SpeakersListPage.SpeakersList.UpdateAllSpeakersAsync();
            }
            catch (Exception ex)
            {
                window.Log(ex.Message);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SpeakersListPage.SpeakersList.SetSingleSelectionMode();
        }
    }
}
