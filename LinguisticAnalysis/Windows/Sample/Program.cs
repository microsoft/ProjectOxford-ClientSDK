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
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Linguistics;
using Microsoft.ProjectOxford.Linguistics.Contract;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Microsoft.ProjectOxford.Linguistics.Sample
{
    class Program
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LinguisticsClient"/> class.
        /// </summary>
        private static readonly LinguisticsClient Client = new LinguisticsClient("Your subscription key");

        /// <summary>
        /// Default jsonserializer settings
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        static void Main(string[] args)
        {
            // List analyzers
            Analyzer[] supportedAnalyzers = null;
            try
            {
                supportedAnalyzers = ListAnalyzers();
                var analyzersAsJson = JsonConvert.SerializeObject(supportedAnalyzers, Formatting.Indented, jsonSerializerSettings);
                Console.WriteLine("Supported analyzers: " + analyzersAsJson);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to list supported analyzers: " + e.ToString());
                Environment.Exit(1);
            }

            // Analyze text with all available analyzers
            var analyzeTextRequest = new AnalyzeTextRequest()
            {
                Language = "en",
                AnalyzerIds = supportedAnalyzers.Select(analyzer => analyzer.Id).ToArray(),
                Text = "Welcome to Microsoft Linguistic Analysis!"
            };

            try
            {
                var analyzeTextResults = AnalyzeText(analyzeTextRequest);
                var resultsAsJson = JsonConvert.SerializeObject(analyzeTextResults, Formatting.Indented, jsonSerializerSettings);
                Console.WriteLine("Analyze text results: " + resultsAsJson);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Failed to list supported analyzers: " + e.ToString());
                Environment.Exit(1);
            }

            Console.ReadKey();
        }

        /// <summary>
        /// List analyzers synchronously.
        /// </summary>
        /// <returns>An array of supported analyzers.</returns>
        private static Analyzer[] ListAnalyzers()
        {
            try
            {
                return Client.ListAnalyzersAsync().Result;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to gather list of analyzers", exception as ClientException);
            }
        }

        /// <summary>
        /// Analyze text synchronously.
        /// </summary>
        /// <param name="request">Analyze text request.</param>
        /// <returns>An array of analyze text result.</returns>
        private static AnalyzeTextResult[] AnalyzeText(AnalyzeTextRequest request)
        {
            try
            {
                return Client.AnalyzeTextAsync(request).Result;
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to analyze text", exception as ClientException);
            }
        }
    }
}
