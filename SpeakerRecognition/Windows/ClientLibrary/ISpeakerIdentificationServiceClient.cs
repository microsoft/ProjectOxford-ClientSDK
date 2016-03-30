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

using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Identification;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.SpeakerRecognition
{
    /// <summary>
    /// An interface for a speaker identification service client related methods
    /// </summary>
    public interface ISpeakerIdentificationServiceClient
    {
        /// <summary>
        /// Identifies a given speaker using the speaker ID and audio stream asynchronously
        /// </summary>
        /// <param name="audioStream">The audio stream to identify</param>
        /// <param name="ids">The list of possible speaker profile IDs to identify from</param>
        /// <exception cref="IdentificationException">Thrown on cases of internal server error, invalid IDs or wrong audio format</exception>
        /// <returns>An object encapsulating the The Url that can be used to query the identification operation status</returns>
        Task<OperationLocation> IdentifyAsync(Stream audioStream, Guid[] ids);

        /// <summary>
        /// Creates a new speaker profile asynchronously
        /// </summary>
        /// <param name="locale">The speaker profile locale</param>
        /// <exception cref="CreateProfileException">Thrown on cases of internal server error or an invalid locale</exception>
        /// <returns>The profile object encapsulating the response object of the create request</returns>
        Task<CreateProfileResponse> CreateProfileAsync(string locale);

        /// <summary>
        /// Deletes a given speaker profile asynchronously
        /// </summary>
        /// <exception cref="DeleteProfileException">Thrown on cases of internal server error on an invalid ID</exception>
        /// <param name="id">The ID of the speaker profile to be deleted</param>
        Task DeleteProfileAsync(Guid id);

        /// <summary>
        /// Retrieves a speaker profile from the service asynchronously
        /// </summary>
        /// <param name="id">The ID of the speaker profile to get</param>
        /// <exception cref="GetProfileException">Thrown in cases of invalid ID or an internal server error</exception>
        /// <returns>The requested profile</returns>
        Task<Profile> GetProfileAsync(Guid id);

        /// <summary>
        /// Gets all speaker profiles from the service asynchronously
        /// </summary>
        /// <exception cref="GetProfileException">Thrown on internal server error</exception>
        /// <returns>An array containing a list of all profiles</returns>
        Task<Profile[]> GetProfilesAsync();

        /// <summary>
        /// Enrolls a speaker profile from an audio stream asynchronously
        /// </summary>
        /// <param name="audioStream">The audio stream to use for enrollment</param>
        /// <param name="id">The speaker profile ID to enroll</param>
        /// <exception cref="EnrollmentException">Thrown in case of invalid audio format, internal server error or an invalid ID</exception>
        /// <returns>An object encapsulating the The Url that can be used to query the enrollment operation status</returns>
        Task<OperationLocation> EnrollAsync(Stream audioStream, Guid id);

        /// <summary>
        /// Gets the enrollment operation status or result asynchronously
        /// </summary>
        /// <param name="location">The Url returned upon calling the enrollment operation</param>
        /// <exception cref="EnrollmentException">Thrown in case of internal server error or an invalid url</exception>
        /// <returns>The enrollment operation object encapsulating the result</returns>
        Task<EnrollmentOperation> CheckEnrollmentStatusAsync(OperationLocation location);

        /// <summary>
        /// Gets the identification operation status or result asynchronously
        /// </summary>
        /// <param name="location">The Url returned upon calling the identification operation</param>
        /// <exception cref="IdentificationException">Thrown in case of internal server error or an url</exception>
        /// <returns>The identification operation object encapsulating the result</returns>
        Task<IdentificationOperation> CheckIdentificationStatusAsync(OperationLocation location);

        /// <summary>
        /// Deletes all enrollments associated with the given speaker identification profile permanently from the service asynchronously
        /// </summary>
        /// <exception cref="ResetEnrollmentsException">Thrown in case of internal server error or an invalid ID</exception>
        /// <param name="id">The speaker ID</param>
        Task ResetEnrollmentsAsync(Guid id);
    }
}
