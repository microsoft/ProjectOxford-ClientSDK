//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
//
// Project Oxford: http://ProjectOxford.ai
//
// ProjectOxford SDK GitHub:
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

using System.Windows;

namespace Microsoft.ProjectOxford.Face
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructors

        /// <summary>
        /// Prevents a default instance of the <see cref="App"/> class from being created
        /// </summary>
        private App()
        {
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Show unhandled exception in message box
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception is FaceAPIException)
            {
                var ex = e.Exception as FaceAPIException;
                MessageBox.Show(ex.ErrorMessage, "Face API Calling Error", MessageBoxButton.OK);
            }
            else
            {
                if (e.Exception.InnerException != null)
                {
                    MessageBox.Show(e.Exception.InnerException.ToString(), "Error", MessageBoxButton.OK);
                }
                else
                {
                    MessageBox.Show(e.Exception.ToString(), "Error", MessageBoxButton.OK);
                }
            }

            e.Handled = true;
        }

        #endregion Methods
    }
}