using System.Linq;
using EmailParserForCalendar.Exceptions;
using EmailParserForCalendar.Google;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public class RescheduledProcessor : IEmailProcessor
    {
        private readonly CalendarClient _calendarClient;

        public RescheduledProcessor(CalendarClient calendarClient)
        {
            _calendarClient = calendarClient;
        }
        
        public void Process(ForwardedEmail email, Database db)
        {
            CalendarEvent existingCalendarEvent = db.CalendarEvents
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault(x => x.Title == email.Title);

            if (existingCalendarEvent == null) // it doesnt exist in google, create new
            {
                try
                {
                    CalendarEvent newCalendarEvent = new CalendarEvent(email);
                    db.CalendarEvents.Add(newCalendarEvent);

                    _calendarClient.AddEvent(newCalendarEvent);
                }
                catch (NoDateFoundException){}
            }
            else
            {
                try
                {
                    //TODO: improve this
                    existingCalendarEvent.EventDate = new CalendarEvent(email).EventDate;
                    existingCalendarEvent.Status = Constants.Active;
                    
                    _calendarClient.ReScheduleEvent(existingCalendarEvent.GoodleId, existingCalendarEvent.EventDate);
                }
                catch (NoDateFoundException)
                {
                    existingCalendarEvent.Status = Constants.Cancelled;
                    _calendarClient.CancelEvent(existingCalendarEvent.GoodleId);
                }
                existingCalendarEvent.RelatedEmails.Add(email);
            }
            db.SaveChanges();
        }
    }
}