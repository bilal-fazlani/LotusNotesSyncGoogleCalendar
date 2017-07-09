using System;

namespace EmailParserForCalendar.Exceptions
{
    internal class RecordSkippedException : Exception
    {
        public RecordSkippedException(string message) : base(message)
        {
            
        }
    }
}