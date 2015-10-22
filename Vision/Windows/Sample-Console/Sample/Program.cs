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
using System.Configuration;
using System.IO;

namespace Microsoft.ProjectOXford.Vision.Sample
{

    /// <summary>
    /// The main program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Show the main menu.
        /// </summary>
        public static void ShowMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Vision API Main Menu");
            Console.WriteLine("1: Analyze Image");
            Console.WriteLine("2: Recognize Text");
            Console.WriteLine("3: Get Thumbnail");
            Console.WriteLine();
        }

        /// <summary>
        /// Check whether the given value string is a valid integer value.
        /// </summary>
        /// <param name="valueStr">The value string.</param>
        /// <param name="value">The result value.</param>
        /// <returns>Return true if the it is valid.</returns>
        public static bool TryParseWidthOrHeight(string valueStr, out int value)
        {
            int resultValue;

            if (int.TryParse(valueStr, out resultValue))
            {
                if (resultValue >= 1 && resultValue <= 1024)
                {
                    value = resultValue;
                    return true;
                }
            }

            value = 0;
            return false;
        }

        /// <summary>
        /// Get a valid image path or Url.
        /// </summary>
        /// <returns>A valid image path or Url</returns>
        public static string GetValidImagePathorUrl()
        {
            string imagePathorUrl = string.Empty;

            Console.Write("Please enter your image path or image url:");
            imagePathorUrl = Console.ReadLine();

            while ((!File.Exists(imagePathorUrl) && !Uri.IsWellFormedUriString(imagePathorUrl, UriKind.Absolute))
                || (Uri.IsWellFormedUriString(imagePathorUrl, UriKind.Absolute) && !imagePathorUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase)))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid image path or url, please make sure the path exists or is well-formed.");
                Console.ResetColor();
                Console.Write("Please enter your image path or image url:");
                imagePathorUrl = Console.ReadLine();
            }

            return imagePathorUrl;
        }

        /// <summary>
        /// Get valid width or height.
        /// </summary>
        /// <param name="widthOrHeight">The width or height text.</param>
        /// <returns>Return the integer value.</returns>
        public static int GetValidWidthorHeight(string widthOrHeight)
        {
            Console.Write(string.Format("Please enter your expected {0}(1~1024):", widthOrHeight));
            string valueStr = Console.ReadLine();
            int value;

            while (!TryParseWidthOrHeight(valueStr, out value))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid value.");
                Console.ResetColor();
                Console.Write(string.Format("Please enter your expected {0}(1~1024):", widthOrHeight));
                valueStr = Console.ReadLine();
            }

            return value;
        }

        /// <summary>
        /// Get result folder.
        /// </summary>
        /// <returns>result Folder.</returns>
        public static string GetResultFolder()
        {
            string folder = string.Empty;

            Console.Write("Please enter result folder:");
            folder = Console.ReadLine();

            while (!Directory.Exists(folder))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid folder path.");
                Console.ResetColor();
                Console.Write("Please enter result folder:");
                folder = Console.ReadLine();
            }

            return folder;
        }

        /// <summary>
        /// Get smart cropping.
        /// </summary>
        /// <returns>smart cropping.</returns>
        public static bool GetSmartCropping()
        {
            string smartCropping = string.Empty;

            Console.Write("Please enter is smart cropping(f/F stands for false, otherwise we will set to true.)?");
            smartCropping = Console.ReadLine();

            return !smartCropping.Equals("f", StringComparison.InvariantCultureIgnoreCase);
        }
        /// <summary>
        /// The enter of this Program.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        public static void Main(string[] args)
        {
            // Get the subscription Key
            string subscriptionKey = ConfigurationManager.AppSettings["subscriptionKey"];
            if (string.IsNullOrWhiteSpace(subscriptionKey))
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("To play this sample, you should firstly get a subscription key and put it into the App.Config file.");
                Console.WriteLine("If you don't have one, please access");
                Console.WriteLine("http://www.projectoxford.ai/doc/general/subscription-key-mgmt");

                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine("Please enter any key......");
                Console.ReadLine();

                return;
            }

            VisionHelper vision = new VisionHelper(subscriptionKey);

            string optionStr = string.Empty;
            string imagePathorUrl = string.Empty;
            bool quit = false;

            while (!quit)
            {
                ShowMainMenu();
                Console.Write("Please choose your option(quit by enter any words):");
                optionStr = Console.ReadLine();

                switch (optionStr)
                {
                    case "1":
                        imagePathorUrl = GetValidImagePathorUrl();
                        vision.AnalyzeImage(imagePathorUrl);
                        break;
                    case "2":
                        imagePathorUrl = GetValidImagePathorUrl();
                        vision.RecognizeText(imagePathorUrl);
                        break;
                    case "3":
                        imagePathorUrl = GetValidImagePathorUrl();
                        int width = GetValidWidthorHeight("width");
                        int height = GetValidWidthorHeight("height");
                        string folder = GetResultFolder();
                        bool smartCropping = GetSmartCropping();
                        vision.GetThumbnail(imagePathorUrl, width, height, smartCropping,folder);
                        break;
                    default:
                        quit = true;
                        break;
                }
            }
        }
    }
}
