namespace EmailParserForCalendar.EventProcessors
{
    public class EventProcessorFactory
    {
        public static IEventProcessor GetEventProcessor(string type)
        {
            switch (type)
            {
                case Constants.Invitation:
                    return new InvitationProcessor();
                case Constants.Rescheduled:
                    return new RescheduledProcessor();
                case Constants.Cancelled:
                    return new CancelledProcessor();
                default:
                    return new NotImplementedProcessor();
                    
            }
        }
    }
}