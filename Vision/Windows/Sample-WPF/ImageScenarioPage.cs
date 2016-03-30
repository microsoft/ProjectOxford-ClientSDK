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


using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Vision.Contract;

namespace VisionAPI_WPF_Samples
{
    /// <summary>
    /// Common UI code for each Image based scenario
    /// </summary>
    public abstract class ImageScenarioPage : Page
    {
        private MainWindow mainWindow => ((MainWindow)Application.Current.MainWindow);

        protected Image PreviewImage { get; set; }

        protected TextBox URLTextBox { get; set; }

        protected string SubscriptionKey
        {
            get
            {
                return mainWindow.ScenarioControl.SubscriptionKey;
            }
        }

        /// <summary>
        /// Perform the work for this scenario
        /// </summary>
        /// <param name="imageUri">The URI of the image to run against the scenario</param>
        /// <param name="upload">Upload the image to Project Oxford if [true]; submit the Uri as a remote url if [false];</param>
        /// <returns></returns>
        protected abstract Task DoWork(Uri imageUri, bool upload);

        private async Task ShowPreviewAndDoWork(Uri imageUri, bool upload)
        {
            try
            {
                // Clear the log
                mainWindow.ScenarioControl.ClearLog();

                // Show the image on the GUI
                BitmapImage bitmapSource = new BitmapImage();
                bitmapSource.BeginInit();
                bitmapSource.CacheOption = BitmapCacheOption.None;
                bitmapSource.UriSource = imageUri;
                bitmapSource.EndInit();
                PreviewImage.Source = bitmapSource;

                // do the actual work for the scenaro
                await DoWork(imageUri, upload);
            }
            catch (Exception exception)
            {
                // Something wen't wrong :(
                Log(exception.ToString());
            }
        }

        protected async void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "Image Files(*.jpg, *.gif, *.bmp, *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";
            bool? result = openDlg.ShowDialog(Application.Current.MainWindow);

            if (!(bool)result)
            {
                return;
            }

            string imageFilePath = openDlg.FileName;
            Uri fileUri = new Uri(imageFilePath);

            await ShowPreviewAndDoWork(fileUri, true);
        }

        protected async void SubmitUriButton_Click(object sender, RoutedEventArgs e)
        {
            Uri imageUri = new Uri(URLTextBox.Text);
            await ShowPreviewAndDoWork(imageUri, false);
        }

        /// <summary>
        /// Logs a message to the parent control
        /// </summary>
        /// <param name="message">The message</param>
        internal void Log(string message)
        {
           mainWindow.ScenarioControl.Log(message);
        }

        /// <summary>
        /// Show Analysis Result
        /// </summary>
        /// <param name="result">Analysis Result</param>
        protected void LogAnalysisResult(AnalysisResult result)
        {
            if (result == null)
            {
                Log("null");
                return;
            }

            if (result.Metadata != null)
            {
                Log("Image Format : " + result.Metadata.Format);
                Log("Image Dimensions : " + result.Metadata.Width + " x " + result.Metadata.Height);
            }

            if (result.ImageType != null)
            {
                string clipArtType;
                switch (result.ImageType.ClipArtType)
                {
                    case 0:
                        clipArtType = "0 Non-clipart";
                        break;
                    case 1:
                        clipArtType = "1 ambiguous";
                        break;
                    case 2:
                        clipArtType = "2 normal-clipart";
                        break;
                    case 3:
                        clipArtType = "3 good-clipart";
                        break;
                    default:
                        clipArtType = "Unknown";
                        break;
                }
                Log("Clip Art Type : " + clipArtType);

                string lineDrawingType;
                switch (result.ImageType.LineDrawingType)
                {
                    case 0:
                        lineDrawingType = "0 Non-LineDrawing";
                        break;
                    case 1:
                        lineDrawingType = "1 LineDrawing";
                        break;
                    default:
                        lineDrawingType = "Unknown";
                        break;
                }
                Log("Line Drawing Type : " + lineDrawingType);
            }


            if (result.Adult != null)
            {
                Log("Is Adult Content : " + result.Adult.IsAdultContent);
                Log("Adult Score : " + result.Adult.AdultScore);
                Log("Is Racy Content : " + result.Adult.IsRacyContent);
                Log("Racy Score : " + result.Adult.RacyScore);
            }

            if (result.Categories != null && result.Categories.Length > 0)
            {
                Log("Categories : ");
                foreach (var category in result.Categories)
                {
                    Log("   Name : " + category.Name + "; Score : " + category.Score);
                }
            }

            if (result.Faces != null && result.Faces.Length > 0)
            {
                Log("Faces : ");
                foreach (var face in result.Faces)
                {
                    Log("   Age : " + face.Age + "; Gender : " + face.Gender);
                }
            }

            if (result.Color != null)
            {
                Log("AccentColor : " + result.Color.AccentColor);
                Log("Dominant Color Background : " + result.Color.DominantColorBackground);
                Log("Dominant Color Foreground : " + result.Color.DominantColorForeground);

                if (result.Color.DominantColors != null && result.Color.DominantColors.Length > 0)
                {
                    string colors = "Dominant Colors : ";
                    foreach (var color in result.Color.DominantColors)
                    {
                        colors += color + " ";
                    }
                    Log(colors);
                }
            }

            if (result.Description != null)
            {
                Log("Description : ");
                foreach (var caption in result.Description.Captions)
                {
                    Log("   Caption : " + caption.Text + "; Confidence : " + caption.Confidence);
                }
                string tags = "   Tags : ";
                foreach (var tag in result.Description.Tags)
                {
                    tags += tag + ", ";
                }
                Log(tags);

            }

            if (result.Tags != null)
            {
                Log("Tags : ");
                foreach (var tag in result.Tags)
                {
                    Log("   Name : " + tag.Name + "; Confidence : " + tag.Confidence + "; Hint : " + tag.Hint);
                }
            }

        }
        
        /// <summary>
        /// Log the result of an analysis in domain result
        /// </summary>
        /// <param name="result"></param>
        protected void LogAnalysisInDomainResult(AnalysisInDomainResult result)
        {
            if (result.Metadata != null)
            {
                Log("Image Format : " + result.Metadata.Format);
                Log("Image Dimensions : " + result.Metadata.Width + " x " + result.Metadata.Height);
            }
            
            if (result.Result != null)
            {
                Log("Result : " + result.Result.ToString());
            }
        }

        /// <summary>
        /// Log text from the given OCR results object.
        /// </summary>
        /// <param name="results">The OCR results.</param>
        protected void LogOcrResults(OcrResults results)
        {
            StringBuilder stringBuilder = new StringBuilder();

            if (results != null && results.Regions != null)
            {
                stringBuilder.Append("Text: ");
                stringBuilder.AppendLine();
                foreach (var item in results.Regions)
                {
                    foreach (var line in item.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            stringBuilder.Append(word.Text);
                            stringBuilder.Append(" ");
                        }

                        stringBuilder.AppendLine();
                    }

                    stringBuilder.AppendLine();
                }
            }

            Log(stringBuilder.ToString());
        }
    }
}
