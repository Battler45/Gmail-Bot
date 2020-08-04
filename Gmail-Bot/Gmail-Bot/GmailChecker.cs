using System;
using System.Linq;
using System.Threading.Tasks;
using Gmail_Bot.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gmail_Bot
{
    public class GmailChecker : IDisposable
    {
        private Respondent Respondent { get; }
        private IMessageFilter MessageFilter { get; }
        private GmailApi GmailApi { get; }
        private BotConfig Config { get; }
        private ILogger<GmailChecker> Logger { get; }

        public GmailChecker(Respondent respondent, IMessageFilter messageFilter, GmailApi gmailApi, IOptions<BotConfig> config, ILogger<GmailChecker> logger)
        {
            if (config?.Value.CountOfRetrieveMessages == null)
                throw new ArgumentNullException();
            Logger = logger;
            (Respondent, MessageFilter, GmailApi, Config) = (respondent, messageFilter, gmailApi, config.Value);
        }

        public async Task RespondUnreadMessagesAsync()
        {
            var messages = await GmailApi.RetrieveLastUnreadMessagesAsync(Config.CountOfRetrieveMessages.Value);
            Logger.LogInformation($"Got {messages.Count} messages");
            var filteredMessages = messages.Where(MessageFilter.Filter).ToList();
            Logger.LogInformation($"Filtered {filteredMessages.Count} messages");
            var responses = filteredMessages.Select(message => Respondent.MakeResponse(message)).ToList();
            Logger.LogInformation($"Prepare {responses.Count} responses");

            foreach (var response in responses)
            {
                await GmailApi.SendAsync(response);
            }
            Logger.LogInformation($"Respond on {responses.Count} messages");
        }

        public void Dispose()
        {
            GmailApi?.Dispose();
        }
    }
}