using System.Linq;
using EmailParserForCalendar.Exceptions;
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
                try
                {
                    CalendarEvent newCalendarEvent = new CalendarEvent(email);
                    db.CalendarEvents.Add(newCalendarEvent);
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
                }
                catch (NoDateFoundException)
                {
                    existingCalendarEvent.Status = Constants.Cancelled;
                }
                existingCalendarEvent.RelatedEmails.Add(email);
            }
            db.SaveChanges();
        }
    }
}