using Google.Apis.Gmail.v1;
using System;
using System.Linq;
using System.Threading.Tasks;
using GmailBot.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GmailBot
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
            using var checker = ServiceProvider.GetService<GmailChecker>();
            await checker.RespondUnreadMessagesAsync();
        }
    }
}