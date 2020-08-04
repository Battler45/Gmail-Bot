using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GmailBot
{
    public class MessageFilterConfig
    {
        [Required]
        [MinLength(1)]
        public List<string> EmailWhiteList { get; set; }
    }
}