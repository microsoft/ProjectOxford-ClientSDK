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
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIDVerificationAPI_WPF_Sample
{
    /// <summary>
    /// A storage helper that's used to persist values to files to be used across pages
    /// </summary>
    internal class IsolatedStorageHelper
    {
        private static IsolatedStorageHelper s_helper;

        private IsolatedStorageHelper()
        {
        }
        /// <summary>
        /// Creates an Instance of the storage helper
        /// </summary>
        /// <returns></returns>
        public static IsolatedStorageHelper getInstance()
        {
            if (s_helper == null)
                s_helper = new IsolatedStorageHelper();
            return s_helper;
        }
        /// <summary>
        /// Reads a value from a given file
        /// </summary>
        /// <param name="filename">The file to read the value from</param>
        /// <returns>String representation of the value</returns>
        public string readValue(string filename)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                try
                {
                    using (var iStream = new IsolatedStorageFileStream(filename, FileMode.Open, isoStore))
                    {
                        using (var reader = new StreamReader(iStream))
                        {
                            return reader.ReadLine();
                        }
                    }
                }
                catch (FileNotFoundException)
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// Writes a value to a given file
        /// </summary>
        /// <param name="fileName">The file to write the value to</param>
        /// <param name="value">The value to be written</param>
        public void writeValue(string fileName, string value)
        {
            using (IsolatedStorageFile isoStore = IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly, null, null))
            {
                using (var oStream = new IsolatedStorageFileStream(fileName, FileMode.Create, isoStore))
                {
                    using (var writer = new StreamWriter(oStream))
                    {
                        writer.WriteLine(value);
                    }
                }
            }
        }
    }
}
