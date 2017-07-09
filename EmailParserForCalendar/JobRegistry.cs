using FluentScheduler;

namespace EmailParserForCalendar
{
    public class JobRegistry : Registry
    {
        public JobRegistry(int hours)
        {
            Schedule<Job>().ToRunNow().AndEvery(hours).Hours();
        }
    }
}