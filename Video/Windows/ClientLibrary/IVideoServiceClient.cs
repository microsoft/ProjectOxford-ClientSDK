// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

using Microsoft.ProjectOxford.Video.Contract;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.Video
{
    /// <summary>
    /// Video interface.
    /// </summary>
    internal interface IVideoServiceClient
    {
        /// <summary>
        /// Create video operation.
        /// </summary>
        /// <param name="video">Video stream.</param>
        /// <param name="operationType">Operation type.</param>
        /// <returns>Video operation created.</returns>
        Task<Operation> CreateOperationAsync(Stream video, OperationType operationType);

        /// <summary>
        /// Create video operation.
        /// </summary>
        /// <param name="video">Video content as byte array.</param>
        /// <param name="operationType">Operation type.</param>
        /// <returns>Video operation created.</returns>
        Task<Operation> CreateOperationAsync(byte[] video, OperationType operationType);

        /// <summary>
        /// Create video operation.
        /// </summary>
        /// <param name="videoUrl">Video url.</param>
        /// <param name="operatioType">Operation type.</param>
        /// <returns>Video operation created.</returns>
        Task<Operation> CreateOperationAsync(string videoUrl, OperationType operatioType);

        /// <summary>
        /// Get video operation result.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns>Operation result.</returns>
        Task<OperationResult> GetOperationResultAsync(Operation operation);

        /// <summary>
        /// Get result video.
        /// </summary>
        /// <param name="url">The result video content url.</param>
        /// <returns>The result video stream.</returns>
        Task<Stream> GetResultVideoAsync(string url);
    }
}
