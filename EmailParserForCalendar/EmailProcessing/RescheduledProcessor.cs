using System.Linq;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public class RescheduledProcessor : IEmailProcessor
    {
        public void Process(ForwardedEmail email, Database db)
        {
            CalendarEvent existingCalendarEvent = db.CalendarEvents
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault(x => x.Title == email.Title);

            if (existingCalendarEvent == null) // it doesnt exist in google, create new
            {
                CalendarEvent newCalendarEvent = new CalendarEvent(email);
                db.CalendarEvents.Add(newCalendarEvent);
            }
            else
            {
                //TODO: improve this
                existingCalendarEvent.EventDate = new CalendarEvent(email).EventDate;
                existingCalendarEvent.RelatedEmails.Add(email);
            }
            db.SaveChanges();
        }
    }
}