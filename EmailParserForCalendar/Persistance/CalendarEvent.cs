using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
            Match match = SujectLineParser.Parse(email.Subject);

            switch (match.Groups[2].Value)
            {
                case Constants.Rescheduled:
                case Constants.Invitation:
                    People = match.Groups[5].Value.Trim();
                    if (string.IsNullOrEmpty(match.Groups[6].Value)) throw new NoDateFoundException();
                    EventDate = Parse(match.Groups[6].Value.Trim(), match.Groups[8].Value.Trim());
                    MiscInformation = match.Groups[9].Value.Trim();
                    break;

                case Constants.Cancelled:
                {
                    break;
                }
                default:
                    throw new NotImplementedException();
            }
            
            From = match.Groups[1].Value.Trim();
            Title = match.Groups[3].Value.Trim();

            //TODO: Add real google Id
            GoodleId = Guid.NewGuid().ToString();
            
            RelatedEmails.Add(email);
        }

        private DateTimeOffset Parse(string dateString, string timeZoneCode)
        {            
            Dictionary<string,TimeSpan> timezones = new Dictionary<string, TimeSpan>()
            {
                ["ZE5B"] = TimeSpan.FromHours(5) + TimeSpan.FromMinutes(30),
                ["EDT"] = -TimeSpan.FromHours(4),
                ["CEDT"] = TimeSpan.FromHours(2),
            };
            
            if(!timezones.ContainsKey(timeZoneCode)) throw new ParsingFailedException(timeZoneCode);
            
            try
            {
                DateTime date = DateTime.ParseExact(dateString, "MMM d hh:mm tt", CultureInfo.InvariantCulture);
                DateTimeOffset offset = new DateTimeOffset(date, timezones[timeZoneCode]);
                return offset;
            }
            catch (FormatException)
            {
                throw new ParsingFailedException(dateString);
            }
        }
        
        [Key]
        public string GoodleId { get; set; }
        
        public string Title { get; set; }
        
        public string From { get; set; }
        
        public string People { get; set; }
        
        public string MiscInformation { get; set; }
        
        public DateTimeOffset EventDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string Status { get; set; } = Constants.Active;
                
        public ICollection<ForwardedEmail> RelatedEmails { get; set; } = new List<ForwardedEmail>();
    }
}