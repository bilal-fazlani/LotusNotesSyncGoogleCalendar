using System;
using System.IO;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace EmailParserForCalendar.Google
{
    public class GoogleServiceFactory
    {
        private static readonly UserCredential Credential = CreateCredential();
        private static readonly string ApplicationName = "EmailParserForCalendar";
        private static readonly BaseClientService.Initializer Initializer = new BaseClientService.Initializer
        {
            HttpClientInitializer = Credential,
            ApplicationName = ApplicationName
        };
        
        private static readonly string[] Scopes = { GmailService.Scope.GmailReadonly };
        
        private static UserCredential CreateCredential()
        {
            UserCredential credential;

            using (FileStream stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "userCredentials.json";

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
            return credential;
        }
        
        public static GmailService CreateGmailService()
        {
            return new GmailService(Initializer);
        }
        
        public static CalendarService CreateCalendarService()
        {
            return new CalendarService(Initializer);
        }
    }
}