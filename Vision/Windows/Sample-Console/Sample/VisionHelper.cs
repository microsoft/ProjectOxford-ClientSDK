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
using System.IO;
using System.Text;

using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace Microsoft.ProjectOXford.Vision.Sample
{

    /// <summary>
    /// The class is used to access vision APIs.
    /// </summary>
    public class VisionHelper
    {
        /// <summary>
        /// The vision service client.
        /// </summary>
        private readonly IVisionServiceClient visionClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="VisionHelper"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public VisionHelper(string subscriptionKey)
        {
            this.visionClient = new VisionServiceClient(subscriptionKey);
        }

        /// <summary>
        /// Analyze the given image.
        /// </summary>
        /// <param name="imagePathOrUrl">The image path or url.</param>
        public void AnalyzeImage(string imagePathOrUrl)
        {
            this.ShowInfo("Analyzing");
            AnalysisResult analysisResult = null;

            string resultStr = string.Empty;
            try
            {
                if (File.Exists(imagePathOrUrl))
                {
                    using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                    {
                        analysisResult = this.visionClient.AnalyzeImageAsync(stream).Result;
                    }
                }
                else if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
                {
                    analysisResult = this.visionClient.AnalyzeImageAsync(imagePathOrUrl).Result;
                }
                else
                {
                    this.ShowError("Invalid image path or Url");
                }
            }
            catch (ClientException e)
            {
                if (e.Error != null)
                {
                    this.ShowError(e.Error.Message);
                }
                else
                {
                    this.ShowError(e.Message);
                }

                return;
            }
            catch (Exception)
            {
                this.ShowError("Some error happened.");
                return;
            }

            this.ShowAnalysisResult(analysisResult);
        }

        /// <summary>
        /// Recognize text from given image.
        /// </summary>
        /// <param name="imagePathOrUrl">The image path or url.</param>
        /// <param name="detectOrientation">if set to <c>true</c> [detect orientation].</param>
        /// <param name="languageCode">The language code.</param>
        public void RecognizeText(string imagePathOrUrl, bool detectOrientation = true, string languageCode = LanguageCodes.AutoDetect)
        {
            this.ShowInfo("Recognizing");
            OcrResults ocrResult = null;
            string resultStr = string.Empty;

            try
            {
                if (File.Exists(imagePathOrUrl))
                {
                    using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                    {
                        ocrResult = this.visionClient.RecognizeTextAsync(stream, languageCode, detectOrientation).Result;
                    }
                }
                else if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
                {
                    ocrResult = this.visionClient.RecognizeTextAsync(imagePathOrUrl, languageCode, detectOrientation).Result;
                }
                else
                {
                    this.ShowError("Invalid image path or Url");
                }
            }
            catch (ClientException e)
            {
                if (e.Error != null)
                {
                    this.ShowError(e.Error.Message);
                }
                else
                {
                    this.ShowError(e.Message);
                }

                return;
            }
            catch (Exception)
            {
                this.ShowError("Some error happened.");
                return;
            }

            this.ShowRetrieveText(ocrResult);
        }

        /// <summary>
        /// Gets thumbnail for given image.
        /// </summary>
        /// <param name="imagePathOrUrl">The image path or url.</param>
        /// <param name="width">Width of the thumbnail. It must be between 1 and 1024.</param>
        /// <param name="height">Height of the thumbnail. It must be between 1 and 1024.</param>
        /// <param name="smartCropping">Whether enable smart cropping.</param>
        /// <param name="resultFolder">result Folder.</param>
        public void GetThumbnail(string imagePathOrUrl, int width, int height, bool smartCropping, string resultFolder)
        {
            this.ShowInfo("Get Thumbnail");
            byte[] thumbnailResult = null;
            string resultStr = string.Empty;

            try
            {
                if (File.Exists(imagePathOrUrl))
                {
                    using (FileStream stream = File.Open(imagePathOrUrl, FileMode.Open))
                    {
                        thumbnailResult = this.visionClient.GetThumbnailAsync(stream, width, height, smartCropping).Result;
                    }
                }
                else if (Uri.IsWellFormedUriString(imagePathOrUrl, UriKind.Absolute))
                {
                    thumbnailResult = this.visionClient.GetThumbnailAsync(imagePathOrUrl, width, height, smartCropping).Result;
                }
                else
                {
                    this.ShowError("Invalid image path or Url");
                }
            }
            catch (ClientException e)
            {
                if (e.Error != null)
                {
                    this.ShowError(e.Error.Message);
                }
                else
                {
                    this.ShowError(e.Message);
                }

                return;
            }
            catch (Exception)
            {
                this.ShowError("Some error happened.");
                return;
            }

            // Write the result to local file
            string filePath = string.Format("{0}\\thumbnailResult_{1}.jpg", resultFolder, DateTime.UtcNow.Ticks.ToString());

            using (BinaryWriter binaryWrite = new BinaryWriter(new FileStream(filePath, FileMode.Create, FileAccess.Write)))
            {
                binaryWrite.Write(thumbnailResult);
            }

            this.ShowResult(string.Format("The result file has been saved to {0}", Path.GetFullPath(filePath)));
        }

        /// <summary>
        /// Retrieve text from the given OCR results object.
        /// </summary>
        /// <param name="results">The OCR results.</param>
        /// <returns>Return the text.</returns>
        private void ShowRetrieveText(OcrResults results)
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

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(stringBuilder.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// Show the working item.
        /// </summary>
        /// <param name="workStr">The work item's string.</param>
        private void ShowInfo(string workStr)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format("{0}......", workStr));
            Console.ResetColor();
        }

        /// <summary>
        /// Show error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        private void ShowError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorMessage);
            Console.ResetColor();
        }

        /// <summary>
        /// Show result message.
        /// </summary>
        /// <param name="resultMessage">The result message.</param>
        private void ShowResult(string resultMessage)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(resultMessage);
            Console.ResetColor();
        }

        /// <summary>
        /// Show Analysis Result
        /// </summary>
        /// <param name="result">Analysis Result</param>
        private void ShowAnalysisResult(AnalysisResult result)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            if (result == null)
            {
                Console.WriteLine("null");
                return;
            }

            if (result.Metadata != null)
            {
                Console.WriteLine("Image Format : " + result.Metadata.Format);
                Console.WriteLine("Image Dimensions : " + result.Metadata.Width + " x " + result.Metadata.Height);
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
                Console.WriteLine("Clip Art Type : " + clipArtType);
                
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
                Console.WriteLine("Line Drawing Type : " + lineDrawingType);
            }


            if (result.Adult != null)
            {
                Console.WriteLine("Is Adult Content : " + result.Adult.IsAdultContent);
                Console.WriteLine("Adult Score : " + result.Adult.AdultScore);
                Console.WriteLine("Is Racy Content : " + result.Adult.IsRacyContent);
                Console.WriteLine("Racy Score : " + result.Adult.RacyScore);
            }

            if (result.Categories != null && result.Categories.Length > 0)
            {
                Console.WriteLine("Categories : ");
                foreach (var category in result.Categories)
                {
                    Console.Write("   Name : " + category.Name);
                    Console.WriteLine("; Score : " + category.Score);
                }
            }

            if (result.Faces != null && result.Faces.Length > 0)
            {
                Console.WriteLine("Faces : ");
                foreach (var face in result.Faces)
                {
                    Console.Write("   Age : " + face.Age);
                    Console.Write("; Gender : " + face.Gender);
                }
            }

            if (result.Color != null)
            {
                Console.WriteLine("AccentColor : " + result.Color.AccentColor);
                Console.WriteLine("Dominant Color Background : " + result.Color.DominantColorBackground);
                Console.WriteLine("Dominant Color Foreground : " + result.Color.DominantColorForeground);

                if (result.Color.DominantColors != null && result.Color.DominantColors.Length > 0)
                {
                    Console.Write("Dominant Colors : ");
                    foreach (var color in result.Color.DominantColors)
                    {
                        Console.Write(color + " ");
                    }
                }
            }

            Console.ResetColor();
        }

    }
}
