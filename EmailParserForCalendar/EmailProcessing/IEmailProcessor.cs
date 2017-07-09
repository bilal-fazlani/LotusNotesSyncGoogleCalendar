using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public interface IEmailProcessor
    {
        void Process(ForwardedEmail email, Database db);
    }
}