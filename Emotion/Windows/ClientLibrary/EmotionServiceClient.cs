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

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ProjectOxford.Common;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion.Contract;
using Newtonsoft.Json;

namespace Microsoft.ProjectOxford.Emotion
{
    public class EmotionServiceClient : ServiceClient, IEmotionServiceClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmotionServiceClient"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        public EmotionServiceClient(string subscriptionKey) : base()
        {
            ApiRoot = "https://api.projectoxford.ai/emotion/v1.0";
            AuthKey = "Ocp-Apim-Subscription-Key";
            AuthValue = subscriptionKey;
        }

        #region IEmotionServiceClient implementations
        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        public async Task<Contract.Emotion[]> RecognizeAsync(String imageUrl)
        {
            return await RecognizeAsync(imageUrl, null);
        }

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageUrl">URL of the image.</param>
        /// <param name="faceRectangles">Array of face rectangles.</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        public async Task<Contract.Emotion[]> RecognizeAsync(String imageUrl, Rectangle[] faceRectangles)
        {
            return await PostAsync<UrlReqeust, Contract.Emotion[]>(GetRecognizeUrl(faceRectangles), new UrlReqeust { url = imageUrl });
        }

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageStream">Stream of the image</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each recognized face.</returns>
        public async Task<Contract.Emotion[]> RecognizeAsync(Stream imageStream)
        {
            return await RecognizeAsync(imageStream, null);            
        }

        /// <summary>
        /// Recognize emotions on faces in an image.
        /// </summary>
        /// <param name="imageStream">Stream of the image</param>
        /// <returns>Async task, which, upon completion, will return rectangle and emotion scores for each face.</returns>        
        public async Task<Contract.Emotion[]> RecognizeAsync(Stream imageStream, Rectangle[] faceRectangles)
        {
            return await PostAsync<Stream, Contract.Emotion[]>(GetRecognizeUrl(faceRectangles), imageStream);
        }

        private string GetRecognizeUrl(Rectangle[] faceRectangles)
        {
            var builder = new StringBuilder("/recognize");
            if (faceRectangles != null && faceRectangles.Length > 0)
            {
                builder.Append("?faceRectangles=");
                builder.Append(string.Join(";", faceRectangles.Select(r => String.Format("{0},{1},{2},{3}", r.Left, r.Top, r.Width, r.Height))));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Recognize emotions on faces in a video.
        /// </summary>
        /// <param name="videoStream">Video stream</param>
        /// <param name="outputStyle">Output data style</param>
        /// <returns>Video operation created</returns>
        public async Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(Stream videoStream)
        {
            var operation = await PostAsync<Stream, VideoEmotionRecognitionOperation>(@"/recognizeInVideo", videoStream);
            return operation;
        }

        /// <summary>
        /// Recognize emotions on faces in a video.
        /// </summary>
        /// <param name="videoBytes">Video stream byte array</param>
        /// <param name="outputStyle">Output data style</param>
        /// <returns>Video operation created</returns>
        public async Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(byte[] videoBytes)
        {
            using (var videoStream = new MemoryStream(videoBytes))
            {
                var operation = await PostAsync<Stream, VideoEmotionRecognitionOperation>(@"/recognizeInVideo", videoStream);
                return operation;
            }
        }

        /// <summary>
        /// Recognize emotions on faces in a video.
        /// </summary>
        /// <param name="videoUrl">Video URL</param>
        /// <param name="outputStyle">Output data style</param>
        /// <returns>Video operation created</returns>
        public async Task<VideoEmotionRecognitionOperation> RecognizeInVideoAsync(string videoUrl)
        {
            var operation = await PostAsync<string, VideoEmotionRecognitionOperation>(@"/recognizeInVideo", videoUrl);
            return operation;
        }

        /// <summary>
        /// Get emotion video operation result.
        /// </summary>
        /// <param name="operation">Opaque operation object, from RecognizeInVideoAsync response.</param>
        /// <returns>
        /// The output type will vary depending on the outputStyle requested.  For example, if you requested <code>VideoOutputStyle.Aggregate</code>
        /// (default), you would get a VideoOperationInfoResult&lt;VideoAggregateRecognitionResult&gt; object.
        /// <code>
        /// var result = await GetOperationResultAsync(operation);
        /// if (result.Status == VideoOperationStatus.Succeeded)
        /// {
        ///     var details = result as VideoOperationInfoResult&lt;VideoAggregateRecognitionResult&gt
        ///     ...
        /// }
        /// </code>
        /// <code>ProcessResult</code>
        /// </returns>
        public async Task<VideoOperationResult> GetOperationResultAsync(VideoEmotionRecognitionOperation operation)
        {
            var wireResult = await GetAsync<string, VideoOperationInfoResult<string>>(operation.Url, null);

            // The wire-result holds the key result information in a string, deserialize it here so clients
            // don't have to invoke JsonConvert.Deserialize() themselves.

            if (wireResult.Status == VideoOperationStatus.Succeeded)
            {
                var aggregateResult = JsonConvert.DeserializeObject<VideoAggregateRecognitionResult>(wireResult.ProcessingResult);
                return new VideoOperationInfoResult<VideoAggregateRecognitionResult>(wireResult, aggregateResult);
            }

            return wireResult;
        }
        #endregion
    }
}
