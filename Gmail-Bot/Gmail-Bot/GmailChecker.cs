using System;
using System.Linq;
using System.Threading.Tasks;
using AutomatedEmailChecker.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutomatedEmailChecker
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
            Logger = logger;
            (Respondent, MessageFilter, GmailApi, Config) = (respondent, messageFilter, gmailApi, config.Value);
        }

        public async Task RespondUnreadMessagesAsync()
        {
            if (Config?.CountOfRetrieveMessages == null)
                throw new ArgumentNullException();
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