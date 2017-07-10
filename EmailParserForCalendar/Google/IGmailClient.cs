using System.Collections.Generic;
using Google.Apis.Gmail.v1.Data;

namespace EmailParserForCalendar.Google
{
    public interface IGmailClient
    {
        IEnumerable<Message> GetRecentMessages();
    }
}