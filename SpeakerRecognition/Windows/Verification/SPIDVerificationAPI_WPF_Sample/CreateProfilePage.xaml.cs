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

// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SPIDVerificationAPI_WPF_Sample
{
    /// <summary>
    /// Interaction logic for CreateProfile.xaml
    /// </summary>
    public partial class CreateProfilePage : Page
    {
        private string _subscriptionKey;
        private VerificationServiceHttpClientHelper _helper;
        /// <summary>
        /// Initialization constructor for the create profile page
        /// </summary>
        public CreateProfilePage()
        {
            InitializeComponent();
            _helper = new VerificationServiceHttpClientHelper();
            //Storage helper for reading subscription keys
            IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
            string speakerId = _storageHelper.readValue(MainWindow.SPEAKER_FILENAME);
            _subscriptionKey = ((MainWindow)Application.Current.MainWindow).SubscriptionKey;
            _helper.SubscriptionKey = _subscriptionKey;
            if (speakerId != null)
            {
                speakerTxt.Text = speakerId;
            }
        }
        private void createBtn_Click(object sender, RoutedEventArgs e)
        {
            createProfile();
        }
        /// <summary>
        /// A method that's used to create a new profile
        /// </summary>
        private async void createProfile()
        {
            setStatus("Creating Profile...");
            try
            {
                Stopwatch sw = Stopwatch.StartNew();
                SpeakerProfile response = await _helper.CreateProfileAsync("en-us");
                sw.Stop();
                setStatus("Profile Created, Elapsed Time: " + sw.Elapsed);
                speakerTxt.Text = response.VerificationProfileId;
                IsolatedStorageHelper _storageHelper = IsolatedStorageHelper.getInstance();
                _storageHelper.writeValue(MainWindow.SPEAKER_FILENAME, response.VerificationProfileId);
            }
            catch (Exception exception)
            {
                setStatus(exception.Message);
            }
        }
        /// <summary>
        /// A helper method that sets the status bar
        /// </summary>
        /// <param name="status">The status message</param>
        private void setStatus(string status)
        {
            ((MainWindow)Application.Current.MainWindow).Log(status);
        }
    }
}
