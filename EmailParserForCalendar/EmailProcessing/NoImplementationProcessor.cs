using EmailParserForCalendar.Exceptions;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.EmailProcessing
{
    public class NoImplementationProcessor : IEmailProcessor
    {
        public void Process(ForwardedEmail email, Database db)
        {
            throw new RecordSkippedException(email.Operation);
        }
    }
}