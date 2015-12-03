using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIDVerificationAPI_WPF_Sample
{
    /// <summary>
    /// A class encapsulating the result returned by the service as a result of enumerating all possible enrollment
    /// and verification phrases
    /// </summary>
    public class PhraseResponse
    {
        /// <summary>
        /// The enrollment or verification phrase
        /// </summary>
        public string Phrase;
    }
}
