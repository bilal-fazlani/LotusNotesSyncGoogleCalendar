using System;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EventProcessors
{
    public class NotImplementedProcessor : IEventProcessor
    {
        public void Process(CalendarEvent calendarEvent)
        {
            throw new NotImplementedException();
        }
    }
}