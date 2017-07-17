using System;
using System.Collections.Generic;
using EmailParserForCalendar.Persistance;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Serilog;

namespace EmailParserForCalendar.Google
{
    public class GmailClient
    {
        private readonly Settings _settings;
        private readonly GmailService _gmailService;
        private readonly ILogger _logger;

        public GmailClient(Settings settings, GmailService gmailService, ILogger logger)
        {
            _settings = settings;
            _gmailService = gmailService;
            _logger = logger;
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
                    _logger.Warning("No messages returned from server");
                    yield break;
                }
                else
                {
                    _logger.Information("{messagesCount} messages fetched from Google " +
                                        "for the last {maxDays} days",
                        messages.Count, _settings.MaxNumerOfDaysToFetchEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex ,"{errorMessage}", ex.Message);
                yield break;
            }

            using(Database db = new Database())
                foreach (var message in messages)
                {
                    _logger.Verbose("Processing {currentIndex}/{messageCount}",currentIndex++, messages.Count);

                    var email = db.ForwardedEmails.Find(message.Id);
                    if (email == null)
                    {
                        yield return GetMessage(message.Id);
                    }
                    else
                    {
                        _logger.Warning("{messageId} was already attempted/processed", message.Id);
                        _logger.Debug("{subject}", email.Subject);
                    }
                }
        }
    }
}