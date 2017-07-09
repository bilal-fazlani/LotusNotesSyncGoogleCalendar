﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
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
            TimeStamp = DateTime.Now;
            SetOperationAndTitle(Subject);
        }

        private void SetOperationAndTitle(string subject)
        {
            Match match = RegexParser.Parse(subject);
            Operation = match.Groups[2].Value.Trim();
            Title = match.Groups[3].Value.Trim();
            Status = Constants.Parsed;
        }
        
        [Key]
        public string GoodleId { get; set; }
        
        public DateTime TimeStamp { get; set; }
        
        public string Subject { get; set; }
        
        public string Operation { get; set; }
        
        public string Title { get; set; }
        
        public string Status { get; set; }
    }
}