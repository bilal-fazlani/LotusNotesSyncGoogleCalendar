using System.Collections.Generic;

namespace EmailParserForCalendar.EmailProcessing
{
    public class EmailProcessorFactory
    {
        private static readonly Dictionary<string, IEmailProcessor> Processors = new Dictionary<string, IEmailProcessor>
        {
            [Constants.Invitation] = new InvitationProcessor(),
            [Constants.Rescheduled] = new RescheduledProcessor(),
            [Constants.Cancelled] = new CancelledProcessor()
        };
        
        public static IEmailProcessor GetEmailProcessor(string operation)
        {
            IEmailProcessor emailProcessor = null;
            Processors.TryGetValue(operation, out emailProcessor);

            return emailProcessor ?? new NoImplementationProcessor();
        }
    }
}