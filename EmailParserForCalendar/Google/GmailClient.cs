using System;
using System.Collections.Generic;
using EmailParserForCalendar.Persistance;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;

namespace EmailParserForCalendar.Google
{
    public class GmailClient
    {
        private readonly Settings _settings;
        private readonly GmailService _gmailService;

        public GmailClient(Settings settings, GmailService gmailService)
        {
            _settings = settings;
            _gmailService = gmailService;
        }
        
        private Message GetMessage(string messageId)
        {
            UsersResource.MessagesResource.GetRequest request = _gmailService.Users.Messages.Get("me", messageId);
            Message message = request.Execute();
            return message;
        }
        
        public IEnumerable<Message> GetRecentMessages()
        {
            int currentIndex = 1;
            IList<Message> messages = null;

            try
            {
                UsersResource.MessagesResource.ListRequest request = _gmailService.Users.Messages.List("me");

                request.MaxResults = _settings.BatchSize;

                request.Q = $"from:{_settings.From} newer_than:{_settings.MaxNumerOfDaysToFetchEmail}d";

                messages = request.Execute().Messages;

                if (messages == null)
                {
                    Console.WriteLine("No messages returned from server");
                    yield break;
                }
                else
                {
                    Console.WriteLine($"{messages.Count} messages fetched from Google for the last {_settings.MaxNumerOfDaysToFetchEmail} days");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                yield break;
            }

            using(Database db = new Database())
                foreach (var message in messages)
                {
                    Console.Write($"{currentIndex++}/{messages.Count} ");
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