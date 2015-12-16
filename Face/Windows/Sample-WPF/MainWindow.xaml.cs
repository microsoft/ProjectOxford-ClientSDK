//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK GitHub:
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
using System.Runtime.CompilerServices;
using System.Windows;

using Microsoft.ProjectOxford.Face.Controls;

using SampleUserControlLibrary;

namespace Microsoft.ProjectOxford.Face
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // You can use the next line to insert your own subscription key, instead of using UI to set license key.
            this.ViewModel = new MainViewModel()
            {
                FaceDetectionDescription = "Locate faces in an image. You can pick an image by 'Choose Image'. Detected faces will be shown on the image by rectangles surrounding the face, and related attributes will be shown in a list.",
                FaceVerificationDescription = "Determine whether two faces belong to the same person. You can pick single face image, the detected face will be shown on the image. Then click 'Verify' to get the verification result.",
                FaceGroupingDescription = "Put similar faces to same group according to appearance similarity. You can pick an image folder for grouping by 'Group', doing this will group all detected faces and shown under Grouping Result.",
                FaceFindSimilarDescription = "Find faces with appearance similarity. You can pick an image folder, all detected faces inside the folder will be treated as candidate. Use 'Open Query Face' to pick the query faces. The result will be list as 'query face's thumbnail'; similar to 'similar faces' thumbnails'.",
                FaceIdentificationDescription = "Tell whom an input face belongs to given a tagged person database. Here we only handle tagged person database in following format: 1). One root folder. 2). Sub-folders are named as person's name. 3). Each person's images are put into their own sub-folder. Pick the root folder, then choose an image to identify, all faces will be shown on the image with the identified person's name.",
            };
            this.DataContext = this.ViewModel;
            this._scenariosControl.SampleScenarioList = new Scenario[]
            {
                new Scenario()
                {
                    PageClass = typeof(FaceDetectionPage),
                    Title = "Face Detection",
                },
                new Scenario()
                {
                    PageClass = typeof(FaceFindSimilarPage),
                    Title = "Face Find Similar",
                },
                new Scenario()
                {
                    PageClass = typeof(FaceGroupingPage),
                    Title = "Face Grouping",
                },
                new Scenario()
                {
                    PageClass = typeof(FaceIdentificationPage),
                    Title = "Face Identification",
                },
                new Scenario()
                {
                    PageClass = typeof(FaceVerificationPage),
                    Title = "Face Verification",
                },
            };
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets view model instance for MainWindow
        /// </summary>
        public MainViewModel ViewModel
        {
            get; private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Log message in main window log pane
        /// </summary>
        /// <param name="format">format string</param>
        /// <param name="args">format arguments</param>
        public static void Log(string format, params object[] args)
        {
            ((MainWindow)Application.Current.MainWindow)._scenariosControl.Log(string.Format(format, args));
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// View model for MainWindow, covers display image, text
        /// </summary>
        public class MainViewModel : INotifyPropertyChanged
        {
            #region Events

            /// <summary>
            /// Implements INotifyPropertyChanged interface
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion Events

            #region Properties

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

            #endregion Properties

            #region Methods

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

            #endregion Methods
        }

        #endregion Nested Types
    }
}