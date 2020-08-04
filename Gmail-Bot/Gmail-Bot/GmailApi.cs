using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Gmail_Bot
{
    public class LazyAsyncSingleton<T>: IDisposable
        where T : class, IDisposable
    {
        private IAsyncFactory<T> Factory { get; set; }
        private T Instance { get; set; }
        private readonly object locker = new object();

        public LazyAsyncSingleton(IAsyncFactory<T> factory)
        {
            Factory = factory;
        }

        public async Task<T> GetInstanceAsync()
        {
            if (Instance != null) return Instance;
            
            Monitor.Enter(locker);
            Instance ??= await Factory.CreateAsync();
            Monitor.Exit(locker);

            return Instance;
        }


        public bool IsCreated()
        {
            return Instance != null;
        }

        public void Dispose()
        {
            Monitor.Enter(locker);
            if (Instance != null)
            {
                Instance.Dispose();
                Instance = null;
            }
            Factory = null;
            Monitor.Exit(locker);
        }
    }


    public interface IAsyncFactory<T>
    {
        Task<T> CreateAsync();
    }


    public class GmailServiceFactory : IAsyncFactory<GmailService>
    {
        private const string CredentialPath = "credentials.json";
        private const string TokenPath = "token.json";
        private async Task<UserCredential> GetCredential(string credentialPath, string tokenPath)
        {
            await using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);
            // The file token.json stores the user's access and refresh tokens, and is created
            // automatically when the authorization flow completes for the first time.
            var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                new[]
                {
                    GmailService.Scope.GmailSend,
                    GmailService.Scope.GmailReadonly
                },
                "user",
                CancellationToken.None,
                new FileDataStore(tokenPath, true));
            return credential;
        }

        public async Task<GmailService> CreateAsync()
        {
            var credential = await GetCredential(CredentialPath, TokenPath);
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential
            });
            return service;
        }
    }

    public sealed class GmailApi : IDisposable
    {
        private LazyAsyncSingleton<GmailService> LazyAsyncSingletonService { get; }
        public GmailApi(IAsyncFactory<GmailService> serviceFactory)
        {
            LazyAsyncSingletonService = new LazyAsyncSingleton<GmailService>(serviceFactory);
        }
        public async Task<List<Message>> RetrieveLastUnreadMessagesAsync(uint count)
        {
            var service = await LazyAsyncSingletonService.GetInstanceAsync();
            var messagesQuery = service.Users.Messages.List(GmailConsts.AuthenticatedUserId);
            messagesQuery.LabelIds = new[]
            {
                GmailConsts.LabelIds.Unread,
                GmailConsts.LabelIds.Inbox,
                GmailConsts.LabelIds.Personal
            };
            messagesQuery.MaxResults = count;
            var messagesData = await messagesQuery.ExecuteAsync();
            var messages = new List<Message>(messagesData.Messages.Count);
            foreach (var messageData in messagesData.Messages)
            {
                var newMessage = await service.Users.Messages.Get(GmailConsts.AuthenticatedUserId, messageData.Id).ExecuteAsync();
                messages.Add(newMessage);
            }
            return messages;
        }

        public async Task<Message> SendAsync(Message message)
        {
            var service = await LazyAsyncSingletonService.GetInstanceAsync();
            return await service.Users.Messages.Send(message, GmailConsts.AuthenticatedUserId).ExecuteAsync();
        }

        public void Dispose()
        {
            LazyAsyncSingletonService.Dispose();
        }
    }
}