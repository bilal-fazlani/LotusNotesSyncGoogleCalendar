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
        private static readonly string ApplicationName = "EmailParserForCalendar";

        private static BaseClientService.Initializer CreateInitialiser(string scope)
        {
            BaseClientService.Initializer initializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = CreateCredential(scope),
                ApplicationName = ApplicationName
            };
            return initializer;
        }
        
        private static UserCredential CreateCredential(string scope)
        {   
            UserCredential credential;

            using (FileStream stream = new FileStream("auth/credentials.client.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = MakeCredPath(scope);

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[]{scope},
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            
            return credential;
        }
        
        public static GmailService CreateGmailService()
        {
            return new GmailService(CreateInitialiser(GmailService.Scope.GmailReadonly));
        }
        
        public static CalendarService CreateCalendarService()
        {
            return new CalendarService(CreateInitialiser(CalendarService.Scope.Calendar));
        }

        private static string MakeCredPath(string scope)
        {
            int lastIndex = scope.LastIndexOf(@"/", StringComparison.Ordinal);
            return "auth/credentials."+scope.Substring(lastIndex + 1);
        }
    }
}