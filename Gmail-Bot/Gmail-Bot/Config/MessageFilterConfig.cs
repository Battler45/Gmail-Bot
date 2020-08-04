using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gmail_Bot
{
    public class MessageFilterConfig
    {
        [Required]
        [MinLength(1)]
        public List<string> EmailWhiteList { get; set; }
    }
}