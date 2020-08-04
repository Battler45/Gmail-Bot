using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutomatedEmailChecker
{
    public class BotConfig
    {
        [Required]
        [MinLength(1)]
        public List<string> MessageRegexes { get; set; }
        [Required]
        public ResponseTemplate ResponseTemplates { get; set; }
        [Required]
        public uint? CountOfRetrieveMessages { get; set; }

        public class ResponseTemplate
        {
            [Required]
            public string Greeting { get; set; }
            [Required]
            public string InsteadRegexFirstGroup { get; set; }
            [Required]
            public string ClosingWords { get; set; }
        }
    }
}