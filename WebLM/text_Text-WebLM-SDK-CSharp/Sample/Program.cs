namespace Microsoft.ProjectOxford.CSharpSamples.WebLM
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.ProjectOxford.Text.WebLM;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Program class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Initialzes a new instance of <see cref="WebLMClient"/> class.
        /// </summary>
        private static readonly WebLMClient Client = new WebLMClient("Your subscription key");

        /// <summary>
        /// Defalut jsonserializer settings
        /// </summary>
        private static JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        /// <summary>
        /// Main Function
        /// </summary>
        /// <param name="args">input args</param>
        static void Main(string[] args)
        {
            ///Which model to use. Only title/anchor/query/body are supported.
            var model = "body";

            ///The order of N-gram. Valid range: [1, 5]
            var order = 5;

            ///Max number of candidates would be returned. Valid range: [1, 20]
            var maxNumOfCandidatesReturned = 5;

            ///Calculate joint probability
            var queries = new string[] { "your queries to calculate joint probability" };
            Console.WriteLine("Calculate joint probabilities:");
            Console.WriteLine(JsonConvert.SerializeObject(queries));
            var result = CalculateJointProbabilities(queries, model, order);

            ///Calculate conditional probabilities
            //var query = new ConditionalProbabilityQuery() { Words = "conditional probability query", Word = "query" };
            //var queries = new ConditionalProbabilityQuery[] { query };
            //Console.WriteLine("Calculate conditional probabilities:");
            //Console.WriteLine(JsonConvert.SerializeObject(queries));
            //var result = CalculateConditionalProbabilities(queries, model, order);

            ///Generate next words
            //var words = "next word is";
            //Console.WriteLine("Generate next words:");
            //Console.WriteLine(JsonConvert.SerializeObject(words));
            //var result = GenerateNextWords(words, model, order, maxNumOfCandidatesReturned);

            ///Break into words
            //var text = "yourtexttobreak";
            //Console.WriteLine("Break into words:");
            //Console.WriteLine(JsonConvert.SerializeObject(text));
            //var result = BreakIntoWords(text, model, order, maxNumOfCandidatesReturned);

            ///List available models
            //Console.WriteLine("List available models:");
            //var result = ListAvailableModels();

            Console.WriteLine(result.Result);
            Console.ReadKey();
        }

        /// <summary>
        /// Calculate Joint Probabilities asynchronously.
        /// </summary>
        /// <param name="queries">Joint probability queries.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <returns>Joint probability response.</returns>
        private static async Task<object> CalculateJointProbabilities(string[] queries, string model, int order = DefaultValues.OrderDefault)
        {
            object resultObj = null;
            try
            {
                resultObj = await Client.CalculateJointProbabilitiesAsync(queries, model, order);
            }
            catch (Exception exception)
            {
                var clientException = exception as ClientException;
                return clientException.Error.Message;
            }

            return JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializerSettings);
        }

        /// <summary>
        /// Calculate Conditional Probabilities asynchronously.
        /// </summary>
        /// <param name="queries">Conditional probability queries.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <returns>Conditional probability response.</returns>
        private static async Task<object> CalculateConditionalProbabilities(ConditionalProbabilityQuery[] queries, string model, int order = DefaultValues.OrderDefault)
        {
            object resultObj = null;
            try
            {
                resultObj = await Client.CalculateConditionalProbabilitiesAsync(queries, model, order);
            }
            catch (Exception exception)
            {
                var clientException = exception as ClientException;
                return clientException.Error.Message;
            }

            return JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializerSettings);
        }

        /// <summary>
        /// Generate next words asynchronously.
        /// </summary>
        /// <param name="words">A string containing a sequence of words from which to generate the list of words likely to follow.</param>
        /// <param name="model">Which model to use</param>
        /// <param name="order">The order of N-gram.</param>
        /// <param name="maxNumOfCandidatesReturned">Max number of candidates would be returned.</param>
        /// <returns>Next word completions response.</returns>
        private static async Task<object> GenerateNextWords(string words, string model, int order = DefaultValues.OrderDefault, int maxNumOfCandidatesReturned = DefaultValues.CandidatesDefault)
        {
            object resultObj = null;
            try
            {
                resultObj = await Client.GenerateNextWordsAsync(words, model, order, maxNumOfCandidatesReturned);
            }
            catch (Exception exception)
            {
                var clientException = exception as ClientException;
                return clientException.Error.Message;
            }

            return JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializerSettings);              
        }

        /// <summary>
        /// Break into words asynchronously.
        /// </summary>
        /// <param name="text">The line of text to break into words.</param>
        /// <param name="model">Which model to use.</param>
        /// <param name="order">The order of N-gram.</param>
        /// <param name="maxNumOfCandidatesReturned">Max number of candidates would be returned.</param>
        /// <returns>Word breaking response.</returns>
        private static async Task<object> BreakIntoWords(string text, string model, int order = DefaultValues.OrderDefault, int maxNumOfCandidatesReturned = DefaultValues.CandidatesDefault)
        {
            object resultObj = null;
            try
            {
                resultObj = await Client.BreakIntoWordsAsync(text, model, order, maxNumOfCandidatesReturned);
            }
            catch (Exception exception)
            {
                var clientException = exception as ClientException;
                return clientException.Error.Message;
            }

            return JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializerSettings);
        }

        /// <summary>
        /// List available models asynchronously.
        /// </summary>
        /// <returns>List available models response.</returns>
        private static async Task<object> ListAvailableModels()
        {
            object resultObj = null;
            try
            {
                resultObj = await Client.ListAvailableModelsAsync();
            }
            catch (Exception exception)
            {
                var clientException = exception as ClientException;
                return clientException.Error.Message;
            }

            return JsonConvert.SerializeObject(resultObj, Formatting.Indented, jsonSerializerSettings);
        }
    }
}
