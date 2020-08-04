using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace GmailBot
{
    public class GmailServiceFactory : IAsyncFactory<GmailService>
    {
        private const string CredentialPath = "credentials.json";
        private const string TokenPath = "token.json";

        private async Task<UserCredential> ReadCredential(string credentialPath, string tokenPath)
        {
            var lol = File.Exists(credentialPath);
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

        private async Task<UserCredential> GetCredential(string credentialPath, string tokenPath)
        {
            if (File.Exists(credentialPath))
                return await ReadCredential(credentialPath, tokenPath);

            throw new ArgumentException("ну, закинь ты gmail's credentials(в папку с проектом)Будь мозжучком!");
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
}