using System.Linq;
using EmailParserForCalendar.Google;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public class CancelledProcessor : IEmailProcessor
    {
        private readonly CalendarClient _calendarClient;

        public CancelledProcessor(CalendarClient calendarClient)
        {
            _calendarClient = calendarClient;
        }
        
        public void Process(ForwardedEmail email, Database db)
        {
            CalendarEvent existingCalendarEvent = db.CalendarEvents
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault(x => x.Title == email.Title);

            if (existingCalendarEvent == null) return;
            
            existingCalendarEvent.RelatedEmails.Add(email);
            existingCalendarEvent.Status = Constants.Cancelled;
            
            db.SaveChanges();


            _calendarClient.CancelEvent(existingCalendarEvent.GoodleId);
        }
    }
}