using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace AutomatedEmailChecker
{
    public class GmailServiceFactory : IAsyncFactory<GmailService>
    {
        private const string CredentialPath = "credentials.json";
        private const string TokenPath = "token.json";

        private async Task<UserCredential> ReadCredential(string credentialPath, string tokenPath)
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

        private async Task<UserCredential> GetCredential(string credentialPath, string tokenPath)
        {
            if (File.Exists(credentialPath))
                return await ReadCredential(credentialPath, tokenPath);

            throw new ArgumentException("ну, закинь ты gmail's credentials(click on \"enable gmail api\" https://developers.google.com/gmail/api/quickstart/dotnet and save it in application folder)Будь мозжучком!");
            // I could create downloader for credentials and simulation browsers's work for user's confirm to oauth2
            // And I thought about 2 ways 
            // 1st way is closed api
            // 2nd way is Selenium, PhantomJS, etc   
            // But there are "Horrible piece of shit"
            // Because there are not reliable at all
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