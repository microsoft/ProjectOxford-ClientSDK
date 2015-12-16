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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace EmotionAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for EmotionDetectionUserControl.xaml
    /// </summary>
    public partial class EmotionDetectionUserControl : UserControl
    {
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register(
                "Image",
                typeof(BitmapImage),
                typeof(EmotionDetectionUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(ImageChangedCallback)));

        public static readonly DependencyProperty EmotionsProperty =
            DependencyProperty.Register(
                "Emotions",
                typeof(Emotion[]),
                typeof(EmotionDetectionUserControl),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(EmotionsChangedCallback)));

        public EmotionDetectionUserControl()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Uri ImageUri
        {
            get; set;
        }

        public BitmapImage Image
        {
            get
            {
                return (BitmapImage)GetValue(ImageProperty);
            }

            set
            {
                SetValue(ImageProperty, value);
            }
        }

        public Emotion[] Emotions
        {
            get
            {
                return (Emotion[])GetValue(EmotionsProperty);
            }

            set
            {
                SetValue(EmotionsProperty, value);
            }
        }

        private static void ImageChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs eventArg)
        {
            EmotionDetectionUserControl userControl = obj as EmotionDetectionUserControl;

            if (userControl != null)
            {
                userControl._image.Source = (BitmapImage)eventArg.NewValue;
                // Remove any previous detection result in the list box
                userControl._resultListBox.ItemsSource = null;
            }
        }

        private static void EmotionsChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs eventArg)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            if (window != null)
            {
                EmotionDetectionUserControl userControl = obj as EmotionDetectionUserControl;

                if (userControl != null)
                {
                    //
                    // Draw face rectangles
                    //
                    window.DrawFaceRectangle(userControl._image, userControl.Image, userControl.Emotions);

                    //
                    // List the result of the emotion detections in a list box with face rectangle images and top 3 emotions and scores.
                    //
                    window.ListEmotionResult(userControl.ImageUri, userControl._resultListBox, userControl.Emotions);
                }
            }
        }
    }
}
