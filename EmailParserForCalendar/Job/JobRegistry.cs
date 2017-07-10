using FluentScheduler;

namespace EmailParserForCalendar.Job
{
    public class JobRegistry : Registry
    {
        public JobRegistry(int hours)
        {
            Schedule<CalendarSyncJob>().ToRunNow().AndEvery(hours).Hours();
        }
    }
}