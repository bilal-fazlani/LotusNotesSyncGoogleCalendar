using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using EmailParserForCalendar.Exceptions;
using Google.Apis.Gmail.v1.Data;

namespace EmailParserForCalendar.Persistance
{
    public class ForwardedEmail
    {
        public ForwardedEmail()
        {
            
        }

        public ForwardedEmail(Message message)
        {
            GoodleId = message.Id;
            Subject = message.Payload.Headers.SingleOrDefault(x => x.Name == "Subject")?.Value;
            SetOperationAndTitle(Subject);
            EmailDate = message.InternalDate.FromEpochMs();
        }

        private void SetOperationAndTitle(string subject)
        {
            try
            {
                Match match = SujectLineParser.Parse(subject);
                Operation = match.Groups[2].Value.Trim();
                Title = match.Groups[3].Value.Trim();
                Status = Constants.Parsed;
            }
            catch (ParsingFailedException)
            {
                Status = Constants.Error;
            }
        }
        
        [Key]
        public string GoodleId { get; set; }
        
        public DateTime? EmailDate { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        public string Subject { get; set; }
        
        public string Operation { get; set; }
        
        public string Title { get; set; }
        
        public string Status { get; set; }
    }
}