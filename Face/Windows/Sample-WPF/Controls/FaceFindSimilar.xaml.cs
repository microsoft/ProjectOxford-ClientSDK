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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;

using ClientContract = Microsoft.ProjectOxford.Face.Contract;

namespace Microsoft.ProjectOxford.Face.Controls
{
    /// <summary>
    /// Interaction logic for FaceDetection.xaml
    /// </summary>
    public partial class FaceFindSimilar : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Description dependency property
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(FaceFindSimilar));

        /// <summary>
        /// Output dependency property
        /// </summary>
        public static readonly DependencyProperty OutputProperty = DependencyProperty.Register("Output", typeof(string), typeof(FaceFindSimilar));

        /// <summary>
        /// Faces collection which will be used to find similar from
        /// </summary>
        private ObservableCollection<Face> _facesCollection = new ObservableCollection<Face>();

        /// <summary>
        /// Find similar results
        /// </summary>
        private ObservableCollection<FindSimilarResult> _findSimilarCollection = new ObservableCollection<FindSimilarResult>();

        /// <summary>
        /// User picked image file path
        /// </summary>
        private string _selectedFile;

        /// <summary>
        /// Query faces
        /// </summary>
        private ObservableCollection<Face> _targetFaces = new ObservableCollection<Face>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FaceFindSimilar" /> class
        /// </summary>
        public FaceFindSimilar()
        {
            InitializeComponent();
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
        /// Gets or sets description
        /// </summary>
        public string Description
        {
            get
            {
                return (string)GetValue(DescriptionProperty);
            }

            set
            {
                SetValue(DescriptionProperty, value);
            }
        }

        /// <summary>
        /// Gets faces collection which will be used to find similar from 
        /// </summary>
        public ObservableCollection<Face> FacesCollection
        {
            get
            {
                return _facesCollection;
            }
        }

        /// <summary>
        /// Gets find similar results
        /// </summary>
        public ObservableCollection<FindSimilarResult> FindSimilarCollection
        {
            get
            {
                return _findSimilarCollection;
            }
        }

        /// <summary>
        /// Gets constant maximum image size for rendering detection result
        /// </summary>
        public int MaxImageSize
        {
            get
            {
                return 300;
            }
        }

        /// <summary>
        /// Gets or sets output
        /// </summary>
        public string Output
        {
            get
            {
                return (string)GetValue(OutputProperty);
            }

            set
            {
                SetValue(OutputProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets user picked image file path
        /// </summary>
        public string SelectedFile
        {
            get
            {
                return _selectedFile;
            }

            set
            {
                _selectedFile = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("SelectedFile"));
                }
            }
        }

        /// <summary>
        /// Gets query faces
        /// </summary>
        public ObservableCollection<Face> TargetFaces
        {
            get
            {
                return _targetFaces;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Pick image and call find similar for each faces detected
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private async void FindSimilar_Click(object sender, RoutedEventArgs e)
        {
            // Show file picker
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files(*.jpg) | *.jpg";
            var filePicker = dlg.ShowDialog();

            if (filePicker.HasValue && filePicker.Value)
            {
                // User picked image
                // Clear previous detection and find similar results
                TargetFaces.Clear();
                FindSimilarCollection.Clear();

                var sw = Stopwatch.StartNew();
                SelectedFile = dlg.FileName;

                var imageInfo = UIHelper.GetImageInfoForRendering(SelectedFile);

                // Detect all faces in the picked image
                using (var fileStream = File.OpenRead(SelectedFile))
                {
                    Output = Output.AppendLine(string.Format("Request: Detecting faces in {0}", SelectedFile));

                    MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                    string subscriptionKey = mainWindow.SubscriptionKey;

                    var faceServiceClient = new FaceServiceClient(subscriptionKey);
                    var faces = await faceServiceClient.DetectAsync(fileStream);

                    // Update detected faces on UI
                    foreach (var face in UIHelper.CalculateFaceRectangleForRendering(faces, MaxImageSize, imageInfo))
                    {
                        TargetFaces.Add(face);
                    }

                    Output = Output.AppendLine(string.Format("Response: Success. Detected {0} face(s) in {0}", faces.Length, SelectedFile));

                    // Find similar faces for each face
                    foreach (var f in faces)
                    {
                        var faceId = f.FaceId;

                        Output = Output.AppendLine(string.Format("Request: Finding similar faces for face {0}", faceId));

                        try
                        {
                            // Call find similar REST API, the result contains all the face ids which similar to the query face
                            var result = await faceServiceClient.FindSimilarAsync(faceId, FacesCollection.Select(ff => Guid.Parse(ff.FaceId)).ToArray());

                            // Update find similar results collection for rendering
                            var gg = new FindSimilarResult();
                            gg.Faces = new ObservableCollection<Face>();
                            gg.QueryFace = new Face()
                                {
                                    ImagePath = SelectedFile,
                                    Top = f.FaceRectangle.Top,
                                    Left = f.FaceRectangle.Left,
                                    Width = f.FaceRectangle.Width,
                                    Height = f.FaceRectangle.Height,
                                    FaceId = faceId.ToString(),
                                };
                            foreach (var fr in result)
                            {
                                gg.Faces.Add(FacesCollection.First(ff => ff.FaceId == fr.FaceId.ToString()));
                            }

                            Output = Output.AppendLine(string.Format("Response: Found {0} similar faces for face {1}", gg.Faces.Count, faceId));

                            FindSimilarCollection.Add(gg);
                        }
                        catch (ClientException ex)
                        {
                            Output = Output.AppendLine(string.Format("Response: {0}. {1}", ex.Error.Code, ex.Error.Message));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Pick image folder and detect all faces in these images
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private async void FolderPicker_Click(object sender, RoutedEventArgs e)
        {
            // Show folder picker
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            var result = dlg.ShowDialog();

            bool forceContinue = false;

            if (result == DialogResult.OK)
            {
                // Enumerate all ".jpg" files in the folder, call detect
                List<Task> tasks = new List<Task>();

                FacesCollection.Clear();
                TargetFaces.Clear();
                FindSimilarCollection.Clear();
                SelectedFile = null;

                // Set the suggestion count is intent to minimum the data preparetion step only,
                // it's not corresponding to service side constraint
                const int SuggestionCount = 10;
                int processCount = 0;

                Output = Output.AppendLine("Request: Preparing, detecting faces in choosen folder.");

                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                string subscriptionKey = mainWindow.SubscriptionKey;

                var faceServiceClient = new FaceServiceClient(subscriptionKey);

                foreach (var img in Directory.EnumerateFiles(dlg.SelectedPath, "*.jpg", SearchOption.AllDirectories))
                {
                    tasks.Add(Task.Factory.StartNew(
                        async (obj) =>
                        {
                            var imgPath = obj as string;

                            // Call detection
                            using (var fStream = File.OpenRead(imgPath))
                            {
                                try
                                {
                                    var faces = await faceServiceClient.DetectAsync(fStream);
                                    return new Tuple<string, ClientContract.Face[]>(imgPath, faces);
                                }
                                catch (ClientException)
                                {
                                    // Here we simply ignore all detection failure in this sample
                                    // You may handle these exceptions by check the Error.Code and Error.Message property for ClientException object
                                    return new Tuple<string, ClientContract.Face[]>(imgPath, null);
                                }
                            }
                        },
                        img).Unwrap().ContinueWith((detectTask) =>
                        {
                            var res = detectTask.Result;
                            if (res.Item2 == null)
                            {
                                return;
                            }

                            foreach (var f in res.Item2)
                            {
                                // Update detected faces on UI
                                this.Dispatcher.Invoke(
                                    new Action<ObservableCollection<Face>, string, ClientContract.Face>(UIHelper.UpdateFace),
                                    FacesCollection,
                                    res.Item1,
                                    f);
                            }
                        }));
                    processCount++;

                    if (processCount >= SuggestionCount && !forceContinue)
                    {
                        var continueProcess = System.Windows.Forms.MessageBox.Show("The images loaded have reached the recommended count, may take long time if proceed. Would you like to continue to load images?", "Warning", MessageBoxButtons.YesNo);
                        if (continueProcess == DialogResult.Yes)
                        {
                            forceContinue = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                await Task.WhenAll(tasks);

                Output = Output.AppendLine(string.Format("Response: Success. Total {0} faces are detected.", FacesCollection.Count));
            }
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Find similar result for UI binding
        /// </summary>
        public class FindSimilarResult : INotifyPropertyChanged
        {
            #region Fields

            /// <summary>
            /// Similar faces collection
            /// </summary>
            private ObservableCollection<Face> _faces;

            /// <summary>
            /// Query face
            /// </summary>
            private Face _queryFace;

            #endregion Fields

            #region Events

            /// <summary>
            /// Implement INotifyPropertyChanged interface
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            #endregion Events

            #region Properties

            /// <summary>
            /// Gets or sets similar faces collection
            /// </summary>
            public ObservableCollection<Face> Faces
            {
                get
                {
                    return _faces;
                }

                set
                {
                    _faces = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Faces"));
                    }
                }
            }

            /// <summary>
            /// Gets or sets query face
            /// </summary>
            public Face QueryFace
            {
                get
                {
                    return _queryFace;
                }

                set
                {
                    _queryFace = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("QueryFace"));
                    }
                }
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}
