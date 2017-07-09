using System.Collections.Generic;
using Google.Apis.Gmail.v1.Data;

namespace EmailParserForCalendar
{
    public interface IGmailClient
    {
        IEnumerable<Message> GetRecentMessages();
    }
}