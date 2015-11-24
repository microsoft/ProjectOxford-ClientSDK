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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SampleUserControlLibrary;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace EmotionAPI_WPF_Samples
{
    public class EmotionResultDisplayItem
    {
        public Uri ImageSource { get; set; }

        public System.Windows.Int32Rect UIRect { get; set; }
        public string Emotion1 { get; set; }
        public string Emotion2 { get; set; }
        public string Emotion3 { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SampleScenarios ScenarioControl
        {
            get
            {
                return _scenariosControl;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            //
            // Initialize SampleScenarios User Control with titles and scenario pages
            //
            _scenariosControl.SampleTitle = "Emotion API";
            _scenariosControl.SampleScenarioList = new Scenario[]
            {
                new Scenario { Title = "Detect emotion using a stream", PageClass = typeof(DetectEmotionUsingStreamPage) },
                new Scenario { Title = "Detect emotion using a URL", PageClass = typeof(DetectEmotionUsingURLPage)},
            };
        }

        public void Log(string message)
        {
            _scenariosControl.Log(message);
        }

        public void LogEmotionResult(Emotion[] emotionResult)
        {
            int emotionResultCount = 0;
            if (emotionResult != null && emotionResult.Length > 0)
            {
                foreach (Emotion emotion in emotionResult)
                {
                    Log("Emotion[" + emotionResultCount + "]");
                    Log("  .FaceRectangle = left: " + emotion.FaceRectangle.Left
                             + ", top: " + emotion.FaceRectangle.Top
                             + ", width: " + emotion.FaceRectangle.Width
                             + ", height: " + emotion.FaceRectangle.Height);

                    Log("  Anger    : " + emotion.Scores.Anger.ToString());
                    Log("  Contempt : " + emotion.Scores.Contempt.ToString());
                    Log("  Disgust  : " + emotion.Scores.Disgust.ToString());
                    Log("  Fear     : " + emotion.Scores.Fear.ToString());
                    Log("  Happiness: " + emotion.Scores.Happiness.ToString());
                    Log("  Neutral  : " + emotion.Scores.Neutral.ToString());
                    Log("  Sadness  : " + emotion.Scores.Sadness.ToString());
                    Log("  Surprise  : " + emotion.Scores.Surprise.ToString());
                    Log("");
                    emotionResultCount++;
                }
            }
            else
            {
                Log("No emotion is detected. This might be due to:\n" +
                    "    image is too small to detect faces\n" +
                    "    no faces are in the images\n" +
                    "    faces poses make it difficult to detect emotions\n" +
                    "    or other factors");
            }
        }

        public void DrawFaceRectangle(Image image, BitmapImage bitmapSource, Emotion[] emotionResult)
        {
            if (emotionResult != null && emotionResult.Length > 0)
            {
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();

                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));

                double dpi = bitmapSource.DpiX;
                double resizeFactor = 96 / dpi;

                foreach (var emotion in emotionResult)
                {
                    Microsoft.ProjectOxford.Common.Rectangle faceRect = emotion.FaceRectangle;

                    drawingContext.DrawRectangle(
                        Brushes.Transparent,
                        new Pen(Brushes.Cyan, 4),
                        new Rect(
                            faceRect.Left * resizeFactor,
                            faceRect.Top * resizeFactor,
                            faceRect.Width * resizeFactor,
                            faceRect.Height * resizeFactor)
                    );
                }

                drawingContext.Close();

                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);

                image.Source = faceWithRectBitmap;
            }
        }


        public void ListEmotionResult(Uri imageUri, ListBox resultListBox, Emotion[] emotionResult)
        {
            if (emotionResult != null)
            {
                EmotionResultDisplay[] resultDisplay = new EmotionResultDisplay[8];
                List<EmotionResultDisplayItem> itemSource = new List<EmotionResultDisplayItem>();
                for (int i = 0; i < emotionResult.Length; i++)
                {
                    Emotion emotion = emotionResult[i];
                    resultDisplay[0] = new EmotionResultDisplay { EmotionString = "Anger", Score = emotion.Scores.Anger };
                    resultDisplay[1] = new EmotionResultDisplay { EmotionString = "Contempt", Score = emotion.Scores.Contempt };
                    resultDisplay[2] = new EmotionResultDisplay { EmotionString = "Disgust", Score = emotion.Scores.Disgust };
                    resultDisplay[3] = new EmotionResultDisplay { EmotionString = "Fear", Score = emotion.Scores.Fear };
                    resultDisplay[4] = new EmotionResultDisplay { EmotionString = "Happiness", Score = emotion.Scores.Happiness };
                    resultDisplay[5] = new EmotionResultDisplay { EmotionString = "Neutral", Score = emotion.Scores.Neutral };
                    resultDisplay[6] = new EmotionResultDisplay { EmotionString = "Sadness", Score = emotion.Scores.Sadness };
                    resultDisplay[7] = new EmotionResultDisplay { EmotionString = "Surprise", Score = emotion.Scores.Surprise };

                    Array.Sort(resultDisplay, delegate (EmotionResultDisplay result1, EmotionResultDisplay result2)
                    {
                        return ((result1.Score == result2.Score) ? 0 : ((result1.Score < result2.Score) ? 1 : -1));
                    });

                    StackPanel itemPanel = new StackPanel();
                    itemPanel.Orientation = Orientation.Horizontal;

                    String[] emotionStrings = new String[3];
                    for (int j = 0; j < 3; j++)
                    {
                        StackPanel emotionPanel = new StackPanel();
                        emotionPanel.Orientation = Orientation.Vertical;

                        TextBlock emotionText = new TextBlock();
                        emotionText.Text = resultDisplay[j].EmotionString + ":" + resultDisplay[j].Score.ToString("0.000000");

                        itemPanel.Children.Add(emotionPanel);
                        emotionStrings[j] = resultDisplay[j].EmotionString + ":" + resultDisplay[j].Score.ToString("0.000000"); ;
                    }
                    itemSource.Add(new EmotionResultDisplayItem
                    {
                        ImageSource = imageUri,
                        UIRect = new Int32Rect(emotion.FaceRectangle.Left, emotion.FaceRectangle.Top, emotion.FaceRectangle.Width, emotion.FaceRectangle.Height),
                        Emotion1 = emotionStrings[0],
                        Emotion2 = emotionStrings[1],
                        Emotion3 = emotionStrings[2]
                    });
                }
                resultListBox.ItemsSource = itemSource;
            }
        }
    }
}
