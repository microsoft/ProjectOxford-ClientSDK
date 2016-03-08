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
using Microsoft.ProjectOxford.Text.WebLM;

namespace Microsoft.ProjectOxford.CSharpSamples.WebLM
{
    /// <summary>
    /// Program class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Initialzes a new instance of <see cref="LMServiceClient"/> class.
        /// </summary>
        private static readonly LMServiceClient s_client = new LMServiceClient("Paste your subscription key here.");

        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">input args</param>
        static void Main(string[] args)
        {
            /// Which model to use. Only title/anchor/query/body are currently supported.
            var model = "body";

            /// The Markov N-gram order to use. If higher than the model's max order, the model's max order is used instead.
            var order = 5;

            /// The maximum number of results to be returned by next word generation and word breaking. The limit is 1000.
            var maxNumCandidates = 5;

            /// List available models.
            var modelsResponse = s_client.ListAvailableModelsAsync().GetAwaiter().GetResult();
            foreach (var modelsResult in modelsResponse.Models)
                Console.WriteLine("Models:  Name={0}  MaxOrder={1}", modelsResult.Name, modelsResult.MaxOrder);

            /// Calculate joint probabilities. (The numbers returned are actualy log-10 probabilities, and therefore always negative.)
            var jpQueries = new string[] { "this is the first string", "the second string" };
            var jpResponse = s_client.CalculateJointProbabilitiesAsync(jpQueries, model, order).GetAwaiter().GetResult();
            foreach (var jpResult in jpResponse.Results)
                Console.WriteLine("Joint probability:  P({0}) = {1}", jpResult.Words, jpResult.Probability);

            /// Calculate conditional probabilities.
            var cpQuery1 = new ConditionalProbabilityQuery() { Words = "world wide", Word = "web" };
            var cpQuery2 = new ConditionalProbabilityQuery() { Words = "one two three", Word = "four" };
            var cpQueries = new ConditionalProbabilityQuery[] { cpQuery1, cpQuery2 };
            var cpResponse = s_client.CalculateConditionalProbabilitiesAsync(cpQueries, model, order).GetAwaiter().GetResult();
            foreach (var cpResult in cpResponse.Results)
                Console.WriteLine("Conditional probability:  P({0}|{1}) = {2}", cpResult.Word, cpResult.Words, cpResult.Probability);

            /// Generate next word completions.
            var nwQuery = "world wide";
            var nwResponse = s_client.GenerateNextWordsAsync(nwQuery, model, order, maxNumCandidates).GetAwaiter().GetResult();
            foreach (var nwResult in nwResponse.Candidates)
                Console.WriteLine("Next words:  {0} -> {1}  {2}", nwQuery, nwResult.Word, nwResult.Probability);

            /// Break a string without spaces into words.
            var wbQuery = "yourtexttobreak";
            var wbResponse = s_client.BreakIntoWordsAsync(wbQuery, model, order, maxNumCandidates).GetAwaiter().GetResult();
            foreach (var wbResult in wbResponse.Candidates)
                Console.WriteLine("Word breaking:  {0} -> {1}", wbResult.Words, wbResult.Probability);

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
