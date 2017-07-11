using System.Collections.Generic;
using EmailParserForCalendar.Google;

namespace EmailParserForCalendar.EmailProcessing
{
    public class EmailProcessorFactory
    {
        private readonly Dictionary<string, IEmailProcessor> _processors;

        public EmailProcessorFactory(CalendarClient calendarClient)
        {
            _processors = new Dictionary<string, IEmailProcessor>
            {
                [Constants.Invitation] = new InvitationProcessor(calendarClient),
                [Constants.Rescheduled] = new RescheduledProcessor(),
                [Constants.Cancelled] = new CancelledProcessor()
            };
        }
        
        public IEmailProcessor GetEmailProcessor(string operation)
        {
            IEmailProcessor emailProcessor = null;
            _processors.TryGetValue(operation, out emailProcessor);

            return emailProcessor ?? new NoImplementationProcessor();
        }
    }
}