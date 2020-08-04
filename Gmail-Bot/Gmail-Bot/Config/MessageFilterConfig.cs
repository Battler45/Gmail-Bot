using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutomatedEmailChecker
{
    public class MessageFilterConfig
    {
        [Required]
        [MinLength(1)]
        public List<string> EmailWhiteList { get; set; }
    }
}