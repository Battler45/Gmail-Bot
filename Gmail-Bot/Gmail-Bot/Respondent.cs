using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Google.Apis.Gmail.v1.Data;
using Microsoft.Extensions.Options;

namespace GmailBot
{
    public class Respondent
    {
        private BotConfig Config { get; }

        public Respondent(IOptions<BotConfig> options)
        {
            Config = options.Value ?? throw new ArgumentNullException();
        }

        private List<Match> GetMatches(string message, IEnumerable<string> messageRegexes)
        {
            return messageRegexes
                .Select(reg => Regex.Matches(message, reg))
                .Aggregate(new List<Match>(), (messageMatches, regexMatches) =>
                {
                    messageMatches.AddRange(regexMatches);
                    return messageMatches;
                })
                .OrderBy(match => match.Index)
                .ToList();
        }

        private string GetPhraseResponse(Match phrase, string phraseFirstPart)
        {
            return //"I see " 
                   //Config.ResponseTemplates.InsteadRegexFirstGroup
                 phraseFirstPart + phrase.Groups.Values.Where(g => g.Name != "0")
                .Aggregate(new StringBuilder(), (gStr, group) => gStr.Append(group));
        }

        public string MakeResponse(string message)
        {
            var matches = GetMatches(message, Config.MessageRegexes);
            var phrasesResponses = matches.Select(m => GetPhraseResponse(m, Config.ResponseTemplates.InsteadRegexFirstGroup))
                .Aggregate(new StringBuilder(), (gStr, group) => gStr.Append(group).Append(Environment.NewLine));

            return //$"Hi, {Environment.NewLine}{Environment.NewLine}" + 
                Config.ResponseTemplates.Greeting +
                phrasesResponses//.Prepend($"Hi, {Environment.NewLine}{Environment.NewLine}")
                .Append(Config.ResponseTemplates.ClosingWords);
        }

        private string MakeResponseSubject(Message message)
        {
            return "Re: " + message.GetSubject();
        }

        public Message MakeResponse(Message message)
        {
            using var responseBuilder = new MessageBuilder()
                .Body(MakeResponse(message.GetBody()))
                .Subject(MakeResponseSubject(message))
                .AddRecipient(message.GetSenderEmailAddress());

            var response = responseBuilder.Build();
            return response;
        }
    }
}