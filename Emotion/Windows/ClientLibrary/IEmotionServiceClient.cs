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

using System.IO;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion.Contract;

namespace Microsoft.ProjectOxford.Emotion
{
    internal interface IEmotionServiceClient
    {
        #region Image operations
        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        Task<Contract.Emotion[]> RecognizeAsync(string imageUrl);

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <param name="faceRectangles">Array of face rectangles.</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each face.</returns>
        Task<Contract.Emotion[]> RecognizeAsync(string imageUrl, Rectangle[] faceRectangles);

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageStream">Stream of the image</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>        
        Task<Contract.Emotion[]> RecognizeAsync(Stream imageStream);

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageStream">Stream of the image</param>
        /// <param name="faceRectangles">Array of face rectangles</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each face.</returns>
        Task<Contract.Emotion[]> RecognizeAsync(Stream imageStream, Rectangle[] faceRectangles);
        #endregion

        #region Video operations
        /// <summary>
        /// Recognize emotions on faces in a video.
        /// </summary>
        /// <param name="videoStream">Video stream</param>
        /// <param name="outputStyle">Output data style</param>
        /// <returns>Video operation created</returns>
        Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(Stream videoStream);

        /// <summary>
        /// Recognize emotions on faces in a video.
        /// </summary>
        /// <param name="videoBytes">Video content as byte array</param>
        /// <param name="outputStyle">Output data style</param>
        /// <returns>Video operation created.</returns>
        Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(byte[] videoBytes);

        /// <summary>
        /// Recognize emotions on faces in a video.
        /// </summary>
        /// <param name="videoUrl">Video URL</param>
        /// <param name="outputStyle">Output data style</param>
        /// <returns>Video operation created</returns>
        Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(string videoUrl);

        /// <summary>
        /// Get video operation result.
        /// </summary>
        /// <param name="operation">The operation</param>
        /// <returns>Operation result.</returns>
        Task<VideoOperationResult> GetOperationResultAsync(VideoEmotionRecognitionOperation operation);
        #endregion
    }
}
