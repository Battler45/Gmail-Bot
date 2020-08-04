using System;
using System.Collections.Generic;
using Gmail_Bot;
using Google.Apis.Gmail.v1.Data;
using Microsoft.Extensions.Options;

namespace Gmail_Bot.Filters
{
    public class SenderEmailFilter: IMessageFilter
    {
        public SenderEmailFilter(IOptions<MessageFilterConfig> options) : this(options.Value.EmailWhiteList)
        {
            if (options?.Value?.EmailWhiteList == null)
            {
                throw new ArgumentNullException("EmailWhiteList is empty");
            }
        }

        private SenderEmailFilter(IEnumerable<string> emailWhitelist) 
            => EmailWhiteList = new HashSet<string>(emailWhitelist);
        private HashSet<string> EmailWhiteList { get; }
        public bool Filter(Message message)
        {
            var email = message.GetSenderEmailAddress();
            return EmailWhiteList.Contains(email);
        }
    }

    public interface IMessageFilter
    {
        bool Filter(Message message);
    }
}
