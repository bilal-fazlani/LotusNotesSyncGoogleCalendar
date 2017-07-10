using System.Linq;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public class CancelledProcessor : IEmailProcessor
    {
        public void Process(ForwardedEmail email, Database db)
        {
            CalendarEvent existingCalendarEvent = db.CalendarEvents
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault(x => x.Title == email.Title);

            if (existingCalendarEvent == null) return;
            
            existingCalendarEvent.RelatedEmails.Add(email);
            existingCalendarEvent.Status = Constants.Cancelled;
            
            db.SaveChanges();
        }
    }
}