using EmailParserForCalendar.Google;
using EmailParserForCalendar.Persistance;
using Google.Apis.Calendar.v3.Data;

namespace EmailParserForCalendar.EmailProcessing
{
    public class InvitationProcessor : IEmailProcessor
    {
        private readonly CalendarClient _calendarClient;

        public InvitationProcessor(CalendarClient calendarClient)
        {
            _calendarClient = calendarClient;
        }
        
        public void Process(ForwardedEmail email, Database db)
        {
            CalendarEvent calendarEvent = new CalendarEvent(email);
            
            AddToGoogle(calendarEvent);
            
            AddToDatabse(db, calendarEvent);
        }

        private Event AddToGoogle(CalendarEvent calendarEvent)
        {
            Event googleEvent = _calendarClient.AddEvent(calendarEvent);
            calendarEvent.GoodleId = googleEvent.Id;
            return googleEvent;
        }

        private static void AddToDatabse(Database db, CalendarEvent calendarEvent)
        {
            db.CalendarEvents.Add(calendarEvent);
            db.SaveChanges();
        }
    }
}