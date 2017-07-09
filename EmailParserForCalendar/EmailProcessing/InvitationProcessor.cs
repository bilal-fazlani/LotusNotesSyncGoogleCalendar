using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public class InvitationProcessor : IEmailProcessor
    {
        public void Process(ForwardedEmail email, Database db)
        {
            CalendarEvent calendarEvent = new CalendarEvent(email);

            db.CalendarEvents.Add(calendarEvent);
            db.SaveChanges();
        }
    }
}