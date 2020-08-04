using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Google.Apis.Gmail.v1.Data;
using Microsoft.IdentityModel.Tokens;

namespace GmailBot
{
    public static class MessageExtensions
    {
        private static string GetEmail(this string textWithEmail)
        {
            const string emailRegexPattern = "<.+>";
            var match = Regex.Match(textWithEmail, emailRegexPattern);
            return match.Success
                ? match.Value.Trim('<', '>')
                : null;
        }

        public static string GetSenderEmailAddress(this Message message)
        {
            var fromHeaderValue = message?.Payload?.Headers?.FirstOrDefault(h => h.Name == GmailConsts.HeaderNames.From)?.Value;
            return fromHeaderValue?.GetEmail();
        }

        public static string GetBody(this Message message)
        {
            
            if (message.Raw != null)
            {
                return message.Raw;
            }

            if (message.Payload?.Body?.Data != null)
            {
                return Base64UrlEncoder.Decode(message.Payload.Body.Data);
            }

            if (message.Payload?.Parts != null 
                && message.Payload.Parts.Count > 0)
            {
                return message.Payload.Parts
                    .Where(p => p?.Body?.Data != null 
                                && p.MimeType == MediaTypeNames.Text.Plain)
                    .Aggregate(new StringBuilder(), (strBuilder, messagePart) =>
                    {
                        strBuilder.Append(Base64UrlEncoder.Decode(messagePart.Body.Data));
                        return strBuilder;
                    }).ToString();
            }

            return null;
        }
        public static string GetSubject(this Message message)
        {
            return message.Payload?.Headers?
                .FirstOrDefault(h => h.Name == GmailConsts.HeaderNames.Subject)?
                .Value;
        }
    }
}
