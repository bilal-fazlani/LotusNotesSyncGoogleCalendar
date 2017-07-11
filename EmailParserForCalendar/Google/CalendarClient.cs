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
                Start = calendarEvent.EventDate.ToEventDateTime(),
                End =  (calendarEvent.EventDate + TimeSpan.FromMinutes(30)).ToEventDateTime()
            };
            var insertRequest = _calendarService.Events.Insert(googleEvent, _emailAddress);
            return insertRequest.Execute();
        }

        public void ReScheduleEvent(string id, DateTimeOffset date)
        {
            _calendarService.Events.Patch(new Event
            {
                Id = id,
                Start = date.ToEventDateTime(),
                End =  (date + TimeSpan.FromMinutes(30)).ToEventDateTime()
            }, _emailAddress, id);
        }

        public Event CancelEvent(string id)
        {
            Event cancelledEvent = _calendarService.Events.Patch(new Event
            {
                Id = id,
                Status = "cancelled"
            }, _emailAddress, id).Execute();

            return cancelledEvent;
        }
    }
}