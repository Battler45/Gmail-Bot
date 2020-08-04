using System;
using System.Net.Mail;
using Google.Apis.Gmail.v1.Data;
using Microsoft.IdentityModel.Tokens;

namespace GmailBot
{
    public sealed class MessageBuilder: IDisposable
    {
        private bool _disposed;
        private MailMessage Message
        {
            get;
        }
        public MessageBuilder()
        {
            Message = new MailMessage();
        }

        public MessageBuilder From(MailAddress address)
        { 
            Message.From = address;
            return this;
        }

        public MessageBuilder Subject(string subject)
        {
            Message.Subject = subject;
            return this;
        }

        public MessageBuilder Body(string body)
        {
            Message.Body = body;
            return this;
        }

        public MessageBuilder AddRecipient(MailAddress address)
        {
            Message.To.Add(address);
            return this;
        }
        public MessageBuilder AddRecipient(string address)
        {
            Message.To.Add(address);
            return this;
        }

        public Message Build()
        {
            var mimeMessage = MimeKit.MimeMessage.CreateFromMailMessage(Message);
            Dispose();
            return new Message
            {
                Raw = Base64UrlEncoder.Encode(mimeMessage.ToString())
            };
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                Message.Dispose();
            }
        }
    }
}