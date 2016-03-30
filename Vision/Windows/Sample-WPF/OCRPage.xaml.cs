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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namesapce for VisionServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace VisionAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for OCRPage.xaml
    /// </summary>
    public partial class OCRPage : ImageScenarioPage
    {
        public OCRPage()
        {
            InitializeComponent();
            this.PreviewImage = _imagePreview;
            this.URLTextBox = _urlTextBox;
            this.languageComboBox.ItemsSource = GetSupportedLanguages();
        }

        /// <summary>
        /// Uploads the image to Project Oxford and performs OCR
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <param name="language">The language code to recognize for</param>
        /// <returns></returns>
        private async Task<OcrResults> UploadAndRecognizeImage(string imageFilePath, string language)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Vision API Service client
            //
            VisionServiceClient VisionServiceClient = new VisionServiceClient(SubscriptionKey);
            Log("VisionServiceClient is created");

            using (Stream imageFileStream = File.OpenRead(imageFilePath))
            {
                //
                // Upload an image and perform OCR
                //
                Log("Calling VisionServiceClient.RecognizeTextAsync()...");
                OcrResults ocrResult = await VisionServiceClient.RecognizeTextAsync(imageFileStream, language);
                return ocrResult;
            }

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Sends a url to Project Oxford and performs OCR
        /// </summary>
        /// <param name="imageUrl">The url to perform recognition on</param>
        /// <param name="language">The language code to recognize for</param>
        /// <returns></returns>
        private async Task<OcrResults> RecognizeUrl(string imageUrl, string language)
        {
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Vision API Service client
            //
            VisionServiceClient VisionServiceClient = new VisionServiceClient(SubscriptionKey);
            Log("VisionServiceClient is created");

            //
            // Perform OCR on the given url
            //
            Log("Calling VisionServiceClient.RecognizeTextAsync()...");
            OcrResults ocrResult = await VisionServiceClient.RecognizeTextAsync(imageUrl, language);
            return ocrResult;

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------
        }

        /// <summary>
        /// Perform the work for this scenario
        /// </summary>
        /// <param name="imageUri">The URI of the image to run against the scenario</param>
        /// <param name="upload">Upload the image to Project Oxford if [true]; submit the Uri as a remote url if [false];</param>
        /// <returns></returns>
        protected override async Task DoWork(Uri imageUri, bool upload)
        {
            _status.Text = "Performing OCR...";

            string languageCode = (languageComboBox.SelectedItem as RecognizeLanguage).ShortCode;

            //
            // Either upload an image, or supply a url
            //
            OcrResults ocrResult;
            if (upload)
            {
                ocrResult = await UploadAndRecognizeImage(imageUri.LocalPath, languageCode);
            }
            else
            {
                ocrResult = await RecognizeUrl(imageUri.AbsoluteUri, languageCode);
            }
            _status.Text = "OCR Done";

            //
            // Log analysis result in the log window
            //
            Log("");
            Log("OCR Result:");
            LogOcrResults(ocrResult);
        }

        private List<RecognizeLanguage> GetSupportedLanguages()
        {
            return new List<RecognizeLanguage>()
            {
                new RecognizeLanguage(){ ShortCode = "unk",     LongName = "AutoDetect"  },
                new RecognizeLanguage(){ ShortCode = "ar",      LongName = "Arabic"  },
                new RecognizeLanguage(){ ShortCode = "zh-Hans", LongName = "Chinese (Simplified)"  },
                new RecognizeLanguage(){ ShortCode = "zh-Hant", LongName = "Chinese (Traditional)"  },
                new RecognizeLanguage(){ ShortCode = "cs",      LongName = "Czech"  },
                new RecognizeLanguage(){ ShortCode = "da",      LongName = "Danish"  },
                new RecognizeLanguage(){ ShortCode = "nl",      LongName = "Dutch"  },
                new RecognizeLanguage(){ ShortCode = "en",      LongName = "English"  },
                new RecognizeLanguage(){ ShortCode = "fi",      LongName = "Finnish"  },
                new RecognizeLanguage(){ ShortCode = "fr",      LongName = "French"  },
                new RecognizeLanguage(){ ShortCode = "de",      LongName = "German"  },
                new RecognizeLanguage(){ ShortCode = "el",      LongName = "Greek"  },
                new RecognizeLanguage(){ ShortCode = "hu",      LongName = "Hungarian"  },
                new RecognizeLanguage(){ ShortCode = "it",      LongName = "Italian"  },
                new RecognizeLanguage(){ ShortCode = "ja",      LongName = "Japanese"  },
                new RecognizeLanguage(){ ShortCode = "ko",      LongName = "Korean"  },
                new RecognizeLanguage(){ ShortCode = "nb",      LongName = "Norwegian"  },
                new RecognizeLanguage(){ ShortCode = "pl",      LongName = "Polish"  },
                new RecognizeLanguage(){ ShortCode = "pt",      LongName = "Portuguese"  },
                new RecognizeLanguage(){ ShortCode = "ro",      LongName = "Romanian" },
                new RecognizeLanguage(){ ShortCode = "ru",      LongName = "Russian"  },
                new RecognizeLanguage(){ ShortCode = "sr-Cyrl", LongName = "Serbian (Cyrillic)" },
                new RecognizeLanguage(){ ShortCode = "sr-Latn", LongName = "Serbian (Latin)" },
                new RecognizeLanguage(){ ShortCode = "sk",      LongName = "Slovak" },
                new RecognizeLanguage(){ ShortCode = "es",      LongName = "Spanish"  },
                new RecognizeLanguage(){ ShortCode = "sv",      LongName = "Swedish"  },
                new RecognizeLanguage(){ ShortCode = "tr",      LongName = "Turkish"  }
            };
        }
    }

    public class RecognizeLanguage
    {
        public string ShortCode { get; set; }
        public string LongName { get; set; }
    }
}
