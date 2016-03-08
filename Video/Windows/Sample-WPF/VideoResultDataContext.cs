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
using System.ComponentModel;

namespace VideoAPI_WPF_Samples
{
    class VideoResultDataContext : INotifyPropertyChanged
    {
        /// <summary>
        /// Uri of original video
        /// </summary>
        public Uri SourceUri
        {
            get { return _sourceUri; }
            set { _sourceUri = value; OnPropertyChanged(nameof(SourceUri)); }
        }

        /// <summary>
        /// Uri of output video
        /// </summary>
        public Uri ResultUri
        {
            get { return _resultUri; }
            set { _resultUri = value; OnPropertyChanged(nameof(ResultUri)); }
        }

        /// <summary>
        /// Indicates whether service is working
        /// </summary>
        public bool IsWorking
        {
            get { return _isWorking; }
            set { _isWorking = value; OnPropertyChanged(nameof(IsWorking)); }
        }

        /// <summary>
        /// Stores result text from service
        /// </summary>
        public string ResultText
        {
            get { return _resultText; }
            set { _resultText = value; OnPropertyChanged(nameof(ResultText)); }
        }

        /// <summary>
        /// Uses this structure to display highlight rectangles at a specific time
        /// </summary>
        public IList<FrameHighlight> FrameHighlights
        {
            get { return _frameHighlights; }
            set { _frameHighlights = value; OnPropertyChanged(nameof(FrameHighlights)); }
        }


        private Uri _sourceUri;
        private Uri _resultUri;
        private bool _isWorking;
        private string _resultText;
        private IList<FrameHighlight> _frameHighlights;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
