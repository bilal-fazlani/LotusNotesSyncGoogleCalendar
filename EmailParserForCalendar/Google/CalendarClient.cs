using System;
using System.Collections.Generic;
using System.Linq;
using EmailParserForCalendar.Persistance;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Gmail.v1;

namespace EmailParserForCalendar.Google
{
    public class CalendarClient
    {
        private readonly CalendarService _calendarService;
        private readonly string _emailAddress;

        public CalendarClient(CalendarService calendarService, GmailService gmailService)
        {
            _calendarService = calendarService;
            _emailAddress = gmailService.Users.GetProfile("me").Execute().EmailAddress;
        }
        
        public Event AddEvent(CalendarEvent calendarEvent)
        {
            Event googleEvent = new Event
            {
                AnyoneCanAddSelf = false,
                Description = $"{calendarEvent.MiscInformation}\n\n\n{calendarEvent.People}",
                GuestsCanModify = false,
                Location = calendarEvent.MiscInformation,
                Summary = calendarEvent.Title,
                ExtendedProperties = new Event.ExtendedPropertiesData
                {
                    Private__  = new Dictionary<string, string>()
                    {
                        ["emailId"] = calendarEvent.RelatedEmails.FirstOrDefault()?.GoodleId
                    }
                },
                Reminders = new Event.RemindersData
                {
                    UseDefault = true
                },
                Start = new EventDateTime
                {
                    DateTime = calendarEvent.EventDate.DateTime,
                    TimeZone = String.Format("{0:zzz}",calendarEvent.EventDate)
                },
                End = new EventDateTime
                {
                    DateTime = calendarEvent.EventDate.DateTime + TimeSpan.FromMinutes(30),
                    TimeZone = String.Format("{0:zzz}",calendarEvent.EventDate)
                }
            };
            var insertRequest = _calendarService.Events.Insert(googleEvent, _emailAddress);
            return insertRequest.Execute();
        }

        public void ReScheduleEvent(string id, DateTimeOffset date)
        {
            
        }

        public void CancelEvent(string id)
        {
            
        }
    }
}