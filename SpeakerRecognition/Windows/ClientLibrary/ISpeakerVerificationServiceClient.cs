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

using Microsoft.ProjectOxford.SpeakerRecognition.Contract.Verification;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.ProjectOxford.SpeakerRecognition
{
    /// <summary>
    /// An interface for a speaker verification service client related methods
    /// </summary>
    public interface ISpeakerVerificationServiceClient
    {
        /// <summary>
        /// Verifies a given speaker using the speaker ID and audio stream asynchronously
        /// </summary>
        /// <param name="audioStream">The stream of audio to be verified</param>
        /// <param name="id">The speaker ID</param>
        /// <exception cref="VerificationException">Thrown in case of invalid ID or internal server error</exception>
        /// <exception cref="EnrollmentException">Thrown in case of invalid audio format</exception>
        /// <returns>A verification object encapsulating the verification result</returns>
        Task<Verification> VerifyAsync(Stream audioStream, Guid id);

        /// <summary>
        /// Creates a new speaker profile asynchronously
        /// </summary>
        /// <param name="locale">The speaker profile locale</param>
        /// <exception cref="CreateProfileException">Thrown in case of internal server error or an invalid locale</exception>
        /// <returns>The Profile object encapsulating the speaker profile response</returns>
        Task<CreateProfileResponse> CreateProfileAsync(string locale);

        /// <summary>
        /// Deletes a given speaker profile asynchronously
        /// </summary>
        /// <exception cref="DeleteProfileException">Thrown in case of internal server error, an invalid ID or failure to delete the profile</exception>
        /// <param name="id">The ID of the speaker profile to be deleted</param>
        Task DeleteProfileAsync(Guid id);

        /// <summary>
        /// Retrieves a given speaker profile as specified by the id param asynchronously
        /// </summary>
        /// <param name="id">The speaker profile ID</param>
        /// <exception cref="GetProfileException">Thrown in case of internal server error or an invalid ID</exception>
        /// <returns>The requested speaker profile</returns>
        Task<Profile> GetProfileAsync(Guid id);

        /// <summary>
        /// Retrieves all available speaker profiles asynchronously
        /// </summary>
        /// <exception cref="GetProfileException">Thrown in case of internal server error</exception>
        /// <returns>An array containing a list of speaker profiles</returns>
        Task<Profile[]> GetProfilesAsync();

        /// <summary>
        /// Gets a list of all available phrases for enrollments asynchronously
        /// </summary>
        /// <param name="locale">The locale of the pharases</param>
        /// <exception cref="PhrasesException">Thrown in case of invalid locale or internal server error</exception>
        /// <returns>An array containing a list of all verification phrases</returns>
        Task<VerificationPhrase[]> GetPhrasesAsync(string locale);

        /// <summary>
        /// Enrolls a new stream for a given speaker asynchronously
        /// </summary>
        /// <param name="audioStream">The stream to enroll</param>
        /// <param name="id">The speaker profile speaker ID</param>
        /// <exception cref="EnrollmentException">Thrown in case of internal server error, wrong ID or an invalid audio format</exception>
        /// <returns>Enrollment object encapsulating the enrollment response</returns>
        Task<Contract.Verification.Enrollment> EnrollAsync(Stream audioStream, Guid id);

        /// <summary>
        /// Deletes all enrollments associated with the given speaker verification profile permanently from the service asynchronously
        /// </summary>
        /// <param name="id">The speaker ID</param>
        /// <exception cref="ResetEnrollmentsException">Thrown in case of invalid ID, failure to reset the profile or an internal server error</exception>
        Task ResetEnrollmentsAsync(Guid id);
    }
}
