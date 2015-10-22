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
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.ProjectOxford.Face
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// FaceSDK Project Name
        /// </summary>
        private string _appName;

        /// <summary>
        /// FaceSDK subscription key
        /// </summary>
        private static string s_subscriptionKey;

        /// <summary>
        /// Microsoft.ProjectOxford.Face project title
        /// </summary>
        private string _title;

        private readonly string IsolatedStorageSubscriptionKeyFileName = "Subscription.txt";
        private readonly string DefaultSubscriptionKeyPromptMessage = "Paste your subscription key here to start";

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // You can use the next line to insert your own subscription key, instead of using UI to set license key.
            // MainWindow.SubscriptionKey = "PasteYourOwnKeyHereAndDeleteTheNextLine";

            SubscriptionKey = GetSubscriptionKeyFromIsolatedStorage();

            AppName = "Microsoft \"Project Oxford\"";
            AppTitle = "Microsoft.ProjectOxford.Face Sample App";
            DataContext = this;

            FaceDetectionDescription = "Locate faces in an image. You can pick an image by 'Choose Image'. Detected faces will be shown on the image by rectangles surrounding the face, and related attributes will be shown in a list.";
            FaceVerificationDescription = "Determine whether two faces belong to the same person. You can pick single face image, the detected face will be shown on the image. Then click 'Verify' to get the verification result.";
            FaceGroupingDescription = "Put similar faces to same group according to appearance similarity. You can pick an image folder for grouping by 'Group', doing this will group all detected faces and shown under Grouping Result.";
            FaceFindSimilarDescription = "Find faces with appearance similarity. You can pick an image folder, all detected faces inside the folder will be treated as candidate. Use 'Open Query Face' to pick the query faces. The result will be list as 'query face's thumbnail'; similar to 'similar faces' thumbnails'.";
            FaceIdentificationDescription = "Tell whom an input face belongs to given a tagged person database. Here we only handle tagged person database in following format: 1). One root folder. 2). Sub-folders are named as person's name. 3). Each person's images are put into their own sub-folder. Pick the root folder, then choose an image to identify, all faces will be shown on the image with the identified person's name.";
            PrivacyStatement = "Microsoft will receive the images you upload and may use them to improve Face API and related services. By submitting an image, you confirm you have consent from everyone in it.";


        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Implement INotifyPropertyChanged interface
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets sample application title
        /// </summary>
        public string AppTitle
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets FaceSDK project name
        /// </summary>
        public string AppName
        {
            get
            {
                return _appName;
            }

            set
            {
                _appName = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets subscription key
        /// </summary>
        public string SubscriptionKey
        {
            get
            {
                return s_subscriptionKey;
            }

            set
            {
                s_subscriptionKey = value;
                OnPropertyChanged<string>();
            }
        }

        /// <summary>
        /// Gets or sets description of face detection
        /// </summary>
        public string FaceDetectionDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of face verification
        /// </summary>
        public string FaceVerificationDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of face grouping 
        /// </summary>
        public string FaceGroupingDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of find similar face
        /// </summary>
        public string FaceFindSimilarDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets description of identification 
        /// </summary>
        public string FaceIdentificationDescription
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets privacy statement 
        /// </summary>
        public string PrivacyStatement
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Navigate to relate statement page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void Footer_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            System.Diagnostics.Process.Start(btn.Tag as string);
        }

        /// <summary>
        /// Navigate to how-to get subscription key help page
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.projectoxford.ai/doc/general/subscription-key-mgmt");
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
        #endregion Methods

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
            } catch (System.Exception exception)
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
            } catch (System.Exception exception)
            {
                MessageBox.Show("Fail to delete subscription key. Error message: " + exception.Message, 
                    "Subscription Key", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
