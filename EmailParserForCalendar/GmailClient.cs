using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using EmailParserForCalendar.Persistance;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace EmailParserForCalendar
{
    public class GmailClient
    {
        private static readonly GmailService Gmail = CreateGmailInstance();
        private static readonly string[] Scopes = { GmailService.Scope.GmailReadonly };
        private static readonly string ApplicationName = "EmailParserForCalendar";
        
        
        private static GmailService CreateGmailInstance()
        {
            UserCredential credential = Authoriseuser();

            GmailService service = new GmailService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName
            });

            return service;
        }
        
        private static UserCredential Authoriseuser()
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
        

        private static Message GetMessage(string messageId)
        {
            UsersResource.MessagesResource.GetRequest request = Gmail.Users.Messages.Get("me", messageId);
            Message message = request.Execute();
            return message;
        }
        
        public static IEnumerable<Message> GetRecentMessages(string lotusNotesSourceEmailAddress, long? maxMessages = 5, int? maxDays = 5)
        {
            IList<Message> messages = null;

            try
            {
                UsersResource.MessagesResource.ListRequest request = Gmail.Users.Messages.List("me");

                request.MaxResults = maxMessages;

                request.Q = $"from:{lotusNotesSourceEmailAddress} newer_than:{maxDays}d";

                messages = request.Execute().Messages;
                
                Console.WriteLine($"{messages.Count} messages fetched from Goodle for the last {maxDays} days");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                yield break;
            }

            using(Database db = new Database())
                foreach (var message in messages)
                {
                    if (db.ForwardedEmails.Find(message.Id) == null)
                    {
                        yield return GetMessage(message.Id);
                    }
                    else
                    {
                        Console.WriteLine($"{message.Id} was already attempted/processed");
                    }
                }
        }
    }
}