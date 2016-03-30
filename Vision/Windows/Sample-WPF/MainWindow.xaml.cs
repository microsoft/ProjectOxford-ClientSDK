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

using System.Windows;
using SampleUserControlLibrary;

namespace VisionAPI_WPF_Samples
{
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
            _scenariosControl.SampleTitle = "Vision API";
            _scenariosControl.SampleScenarioList = new Scenario[]
            {
                new Scenario { Title = "Analyze Image", PageClass = typeof(AnalyzePage) },
                new Scenario { Title = "Analyze Image with Domain Model", PageClass = typeof(AnalyzeInDomainPage) },
                new Scenario { Title = "Describe Image", PageClass = typeof(DescribePage) },
                new Scenario { Title = "Generate Tags", PageClass = typeof(TagsPage) },
                new Scenario { Title = "Recognize Text (OCR)", PageClass = typeof(OCRPage) },
                new Scenario { Title = "Get Thumbnail", PageClass = typeof(ThumbnailPage) },
            };
        }
    }
}
