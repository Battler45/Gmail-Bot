using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace GmailBot
{
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