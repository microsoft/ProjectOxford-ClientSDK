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

using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VideoAPI_WPF_Samples
{
    static class Helpers
    {
        public static string SubscriptionKey
        {
            get
            {
                MainWindow window = (MainWindow)Application.Current.MainWindow;
                return window._scenariosControl.SubscriptionKey;
            }
        }

        public static string PickFile()
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "Video files (*.mp4, *.mov, *.wmv)|*.mp4;*.mov;*.wmv";

            bool? result = openDlg.ShowDialog(window);
            if (!result.GetValueOrDefault(false)) return null;

            return openDlg.FileName;
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public static string FormatJson<T>(string json)
        {
            return JsonConvert.SerializeObject(FromJson<T>(json), Formatting.Indented);
        }

        public static void Log(string identifier, string message, params object[] args)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            window.Log(string.Format("[{0}] {1}", identifier, string.Format(message, args)));
        }
    }
}
