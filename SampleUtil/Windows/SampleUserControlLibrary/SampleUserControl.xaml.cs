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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SampleUserControlLibrary
{
    public class Scenario
    {
        public string Title
        {
            set;
            get;
        }

        public Type PageClass
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SampleScenarios : UserControl
    {
        public static DependencyProperty SampleTitleProperty =
            DependencyProperty.Register("SampleTitle", typeof(string), typeof(SampleScenarios));

        public static DependencyProperty SampleScenarioListProperty =
            DependencyProperty.Register("SampleScenarioList", typeof(Scenario[]), typeof(SampleScenarios));

        private SubscriptionKeyPage _subscriptionPage;

        public string SampleTitle
        {
            get { return (string)GetValue(SampleTitleProperty); }
            set { SetValue(SampleTitleProperty, value); }
        }

        public Scenario[] SampleScenarioList
        {
            get { return (Scenario[])GetValue(SampleScenarioListProperty); }
            set
            {
                SetValue(SampleScenarioListProperty, value);
                _scenarioListBox.ItemsSource = SampleScenarioList;
            }
        }

        /// <summary>
        /// Gets or sets the disclaimer
        /// </summary>
        public string Disclaimer
        {
            get { return _disclaimerTextBlock.Text; }
            set { _disclaimerTextBlock.Text = value; }
        }

        public string SubscriptionKey
        {
            get;
            set;
        }

        public SampleScenarios()
        {
            InitializeComponent();
            _subscriptionPage = new SubscriptionKeyPage(this);
            SubscriptionKey = _subscriptionPage.SubscriptionKey;

            SampleTitle = "Replace SampleNames with SampleScenarios.SampleTitle property";

            SampleScenarioList = new Scenario[]
            {
                new Scenario { Title = "Scenario 1: Replace items using SampleScenarios.ScenarioList" },
                new Scenario { Title = "Scenario 2: Replace items using SampleScenarios.ScenarioList" }
            };

            _scenarioListBox.ItemsSource = SampleScenarioList;

            _scenarioFrame.Navigate(_subscriptionPage);
        }

        public void Log(string logMessage)
        {
            if (String.IsNullOrEmpty(logMessage) || logMessage == "\n")
            {
                _logTextBox.Text += "\n";
            }
            else
            {
                string timeStr = DateTime.Now.ToString("HH:mm:ss.ffffff");
                string messaage = "[" + timeStr + "]: " + logMessage + "\n";
                _logTextBox.Text += messaage;
            }
            _logTextBox.ScrollToEnd();
        }

        public void ClearLog()
        {
            _logTextBox.Text = "";
        }

        private void ScenarioChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox scenarioListBox = sender as ListBox;
            Scenario scenario = scenarioListBox.SelectedItem as Scenario;
            ClearLog();

            if (scenario != null)
            {
                Page page = Activator.CreateInstance(scenario.PageClass) as Page;
                page.DataContext = this.DataContext;
                _scenarioFrame.Navigate(page);
            }
        }

        private void SubscriptionManagementButton_Click(object sender, RoutedEventArgs e)
        {
            _scenarioFrame.Navigate(_subscriptionPage);
            // Reset the selection so that we can get SelectionChangedEvent later.
            _scenarioListBox.SelectedIndex = -1;
        }
    }

    public class ScenarioBindingConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Scenario s = value as Scenario;
            if (s != null)
            {
                return s.Title;
            }
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
