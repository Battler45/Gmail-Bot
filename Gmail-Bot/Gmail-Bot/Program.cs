using Google.Apis.Gmail.v1;
using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Gmail_Bot.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MailKit.Net.Imap;
using MailKit.Security;

namespace Gmail_Bot
{
    class Program
    {
        private static IServiceProvider ServiceProvider { get; }
        private static IConfiguration Configuration { get; }
        static Program()
        {
            var serviceCollection = new ServiceCollection();
            #region logger
            serviceCollection.AddLogging(builder => builder.AddConsole());
            #endregion
            #region config
            Configuration = new ConfigurationBuilder()
                .AddJsonFile(@"appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            #endregion
            #region options

            serviceCollection.AddOptions<MessageFilterConfig>()
                .Bind(Configuration.GetSection(nameof(MessageFilterConfig)))
                .ValidateDataAnnotations();
            serviceCollection.AddOptions<BotConfig>()
                .Bind(Configuration.GetSection(nameof(BotConfig)))
                .ValidateDataAnnotations()
                //add nested validation 
                .Validate(config => config.ResponseTemplates.Greeting != null
                                    && config.ResponseTemplates.ClosingWords != null
                                    && config.ResponseTemplates.InsteadRegexFirstGroup != null);
            //serviceCollection.AddOptions();

            //serviceCollection.Configure<MessageFilterConfig>(Configuration.GetSection(nameof(MessageFilterConfig)));
            //serviceCollection.Configure<BotConfig>(Configuration.GetSection(nameof(BotConfig)));
            #endregion

            serviceCollection.AddSingleton<IMessageFilter, SenderEmailFilter>();
            serviceCollection.AddSingleton<Respondent>();
            serviceCollection.AddSingleton<IAsyncFactory<GmailService>, GmailServiceFactory>();
            serviceCollection.AddSingleton<GmailApi>();
            serviceCollection.AddSingleton<GmailChecker>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private static async Task Main()
        {
            /*
            using var bot = ServiceProvider.GetService<GmailApi>();

            var botConfig = ServiceProvider.GetService<IOptions<BotConfig>>();


            var messages = await bot.RetrieveLastUnreadMessagesAsync(botConfig.Value.CountOfRetrieveMessages.Value);


            var filter = ServiceProvider.GetService<IMessageFilter>();
            messages = messages.Where(filter.Filter).ToList();
            */

            /*

            using var checker = ServiceProvider.GetService<GmailChecker>();
            await checker.RespondUnreadMessagesAsync();
            using (var client = new ImapClient())
            {
                client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                client.Authenticate("ushiromiya7@gmail.com", "53mp3rf1d3l15");

                // do stuff...

                client.Disconnect(true);
            }
            */
        }
    }
}