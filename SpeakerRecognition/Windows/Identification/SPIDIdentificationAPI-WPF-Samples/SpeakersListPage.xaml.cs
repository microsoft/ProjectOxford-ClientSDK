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

namespace SPIDIdentificationAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for SpeakersListPage.xaml
    /// </summary>
    public partial class SpeakersListPage : Page
    {
        private bool _speakersLoaded = false;

        private static SpeakersListPage s_speakersList = new SpeakersListPage();

        /// <summary>
        /// Represents the only inastance of the Speakers List Page
        /// </summary>
        public static SpeakersListPage SpeakersList
        {
            get
            {
                return s_speakersList;
            }
        }

        private SpeakersListPage()
        {
            InitializeComponent();

            MainWindow window = (MainWindow)Application.Current.MainWindow;
        }

        /// <summary>
        /// Adds a speaker profile to the speakers list
        /// </summary>
        /// <param name="speaker">The speaker profile to add</param>
        public void AddSpeaker(IdentificationProfile speaker)
        {
            _speakersListView.Items.Add(speaker);
        }

        /// <summary>
        /// Retreivs all the speakers asynchronously and adds them to the list
        /// </summary>
        /// <returns>Task to track the status of the asynchronous task.</returns>
        public async Task UpdateAllSpeakersAsync()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            try
            {
                IdentificationServiceHttpClientHelper requestHelper = new IdentificationServiceHttpClientHelper(window.ScenarioControl.SubscriptionKey);
                window.Log("Retreiving All Profiles...");
                List<IdentificationProfile> allProfiles = await requestHelper.GetAllProfilesAsync();
                window.Log("All Profiles Retreived.");
                _speakersListView.Items.Clear();
                foreach (IdentificationProfile profile in allProfiles)
                    AddSpeaker(profile);
                _speakersLoaded = true;
            }
            catch (Exception ex)
            {
                window.Log(ex.Message);
            }
        }

        private async void _UpdateBtn_Click(object sender, RoutedEventArgs e)
        {
            await UpdateAllSpeakersAsync();
        }

        public void SetSingleSelectionMode()
        {
            _speakersListView.SelectionMode = SelectionMode.Single;
        }

        public void SetMultipleSelectionMode()
        {
            _speakersListView.SelectionMode = SelectionMode.Multiple;
        }

        public IdentificationProfile[] GetSelectedProfiles()
        {
            if (_speakersListView.SelectedItems.Count == 0)
                throw new Exception("Error: No Speakers Selected.");
            IdentificationProfile[] result = new IdentificationProfile[_speakersListView.SelectedItems.Count];
            for (int i = 0; i < result.Length; i++)
                result[i] = _speakersListView.SelectedItems[i] as IdentificationProfile;
            return result;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_speakersLoaded == false)
            {
                await UpdateAllSpeakersAsync();
            }
        }
    }
}
