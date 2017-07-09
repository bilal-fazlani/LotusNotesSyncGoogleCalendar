using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmailParserForCalendar.Exceptions;

namespace EmailParserForCalendar.Persistance
{
    public class CalendarEvent
    {
        public CalendarEvent()
        {
            
        }
        
        public CalendarEvent(ForwardedEmail email)
        {
            Match match = RegexParser.Parse(email.Subject);

            switch (match.Groups[2].Value)
            {
                case Constants.Rescheduled:
                case Constants.Invitation:
                {
                    People = match.Groups[4].Value.Trim();
                    //TODO: extract timezone
                    EventDate = DateTimeOffset.Parse(match.Groups[5].Value.Trim());
                    MiscInformation = match.Groups[8].Value;
                    break;
                }

                case Constants.Cancelled:
                {
                    break;
                }
                default:
                    throw new RecordSkippedException(match.Groups[2].Value);
            }
            
            From = match.Groups[1].Value.Trim();
            Title = match.Groups[3].Value.Trim();
            
            RelatedEmails.Add(email);
        }
        
        public long Id { get; set; }
        
        public string GoodleId { get; set; }
        
        public string Title { get; set; }
        
        public string From { get; set; }
        
        public string People { get; set; }
        
        public string MiscInformation { get; set; }
        
        public DateTimeOffset EventDate { get; set; }
                
        public List<ForwardedEmail> RelatedEmails { get; set; } = new List<ForwardedEmail>();
    }
}