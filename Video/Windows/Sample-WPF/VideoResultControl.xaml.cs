// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Project Oxford: http://ProjectOxford.ai
// 
// Project Oxford SDK GitHub:
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VideoAPI_WPF_Samples
{
    /// <summary>
    /// Interaction logic for VideoResultControl.xaml
    /// </summary>
    public partial class VideoResultControl : UserControl
    {
        private static readonly Brush HighlightRectangleBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private static readonly int HighlightRectangleThickness = 2;
        private static readonly TimeSpan HighlightTimerInterval = TimeSpan.FromSeconds(0.03);


        public static DependencyProperty SourceUriProperty =
            DependencyProperty.Register("SourceUri", typeof(Uri), typeof(VideoResultControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, VideoSourceChanged));
        public static DependencyProperty ResultUriProperty =
            DependencyProperty.Register("ResultUri", typeof(Uri), typeof(VideoResultControl));
        public static DependencyProperty ResultTextProperty =
            DependencyProperty.Register("ResultText", typeof(string), typeof(VideoResultControl));
        public static DependencyProperty IsWorkingProperty =
            DependencyProperty.Register("IsWorking", typeof(bool), typeof(VideoResultControl));
        public static DependencyProperty IsVideoResultProperty =
            DependencyProperty.Register("IsVideoResult", typeof(bool), typeof(VideoResultControl));
        public static DependencyProperty FrameHighlightsProperty =
            DependencyProperty.Register("FrameHighlights", typeof(IList<FrameHighlight>), typeof(VideoResultControl));

        private readonly DispatcherTimer _highlightTimer = new DispatcherTimer();
        private int _highlightIndex;

        public VideoResultControl()
        {
            InitializeComponent();

            _highlightTimer.Interval = HighlightTimerInterval;
            _highlightTimer.Tick += HighlightTimerOnTick;
        }

        /// <summary>
        /// Represents the video Uri of orginal video player (left panel)
        /// </summary>
        public Uri SourceUri
        {
            get { return (Uri)GetValue(SourceUriProperty); }
            set { SetValue(SourceUriProperty, value); }
        }

        /// <summary>
        /// Represents the video Uri of result video player (right panel)
        /// </summary>
        public Uri ResultUri
        {
            get { return (Uri)GetValue(ResultUriProperty); }
            set { SetValue(ResultUriProperty, value); }
        }

        /// <summary>
        /// Represents the text of result text box (right panel)
        /// </summary>
        public string ResultText
        {
            get { return (string)GetValue(ResultTextProperty); }
            set { SetValue(ResultTextProperty, value); }
        }

        /// <summary>
        /// Indicates whether to show a waiting screen and disable all operations
        /// </summary>
        public bool IsWorking
        {
            get { return (bool)GetValue(IsWorkingProperty); }
            set { SetValue(IsWorkingProperty, value); }
        }

        /// <summary>
        /// Indicates whether to show result video player, otherwise, result text box is shown
        /// </summary>
        public bool IsVideoResult
        {
            get { return (bool)GetValue(IsVideoResultProperty); }
            set { SetValue(IsVideoResultProperty, value); }
        }

        /// <summary>
        /// A sequence of time frames with highlight information, which will be used to draw during video playing
        /// </summary>
        public IList<FrameHighlight> FrameHighlights
        {
            get { return (IList<FrameHighlight>)GetValue(FrameHighlightsProperty); }
            set { SetValue(FrameHighlightsProperty, value); }
        }

        private void ButtonPlay_OnClick(object sender, RoutedEventArgs e)
        {
            StopVideo();
            PlayVideo();
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            StopVideo();
        }

        private void PlayVideo()
        {
            originalVideo.Play();
            resultVideo.Play();

            Debug.Assert(!_highlightTimer.IsEnabled);

            if (FrameHighlights != null && FrameHighlights.Count > 0)
            {
                // Creates highlight controls before playing
                int numOfRects = FrameHighlights.First().HighlightRects.Length;

                for (int i = 0; i < numOfRects; i++)
                {
                    rectangleAreas.Children.Add(new Rectangle()
                    {
                        Visibility = Visibility.Hidden,
                        StrokeThickness = HighlightRectangleThickness,
                        Stroke = HighlightRectangleBrush
                    });
                }

                _highlightIndex = 0;
                _highlightTimer.Start();
            }
        }

        private void StopVideo()
        {
            originalVideo.Stop();
            resultVideo.Stop();
            _highlightTimer.Stop();
            rectangleAreas.Children.Clear();
        }

        private void HighlightTimerOnTick(object sender, EventArgs eventArgs)
        {
            double currentSecond = originalVideo.Position.TotalSeconds;

            // finds the neareast time frame
            while (_highlightIndex < FrameHighlights.Count && FrameHighlights[_highlightIndex].Time <= currentSecond)
                _highlightIndex++;

            if (_highlightIndex > FrameHighlights.Count) return;
            if (_highlightIndex == 0) return;

            Rect[] positions = FrameHighlights[_highlightIndex - 1].HighlightRects;
            for (int i = 0; i < positions.Length; i++)
            {
                // relayouts highlight controls based on video player size
                Rectangle rect = (Rectangle) rectangleAreas.Children[i];
                rect.Visibility = Visibility.Hidden;

                double w = originalVideoHolder.ActualWidth;
                double h = originalVideoHolder.ActualHeight;
                double vw = (double) originalVideo.NaturalVideoWidth;
                double vh = (double) originalVideo.NaturalVideoHeight;

                if (h > 0 && vh > 0)
                {
                    double vr = vw/vh;
                    double offsetX = Math.Max((w - h*vr)/2, 0);
                    double offsetY = Math.Max((h - w/vr)/2, 0);

                    double realWidth = w - 2*offsetX;
                    double realHeight = h - 2*offsetY;

                    Canvas.SetLeft(rect, offsetX + positions[i].X*realWidth);
                    Canvas.SetTop(rect, offsetY + positions[i].Y*realHeight);
                    rect.Width = positions[i].Width*realWidth;
                    rect.Height = positions[i].Height*realHeight;
                    rect.Visibility = Visibility.Visible;
                }
            }
        }

        private static void VideoSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            VideoResultControl thiz = (VideoResultControl)obj;
            thiz.StopVideo();
        }
    }

    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return ((Visibility)value == Visibility.Visible);
        }
    }

    public class InvertBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return ((Visibility)value != Visibility.Visible);
        }
    }

    public class FrameHighlight
    {
        /// <summary>
        /// Start time (in seconds) of the frame
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Rectangles of highlight regions in the frame
        /// </summary>
        public Rect[] HighlightRects { get; set; }
    }
}
