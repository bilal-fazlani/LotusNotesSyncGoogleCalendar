using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EventProcessors
{
    public interface IEventProcessor
    {
        void Process(CalendarEvent calendarEvent);
    }
}