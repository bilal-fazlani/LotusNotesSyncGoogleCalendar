using System;

namespace EmailParserForCalendar.Exceptions
{
    internal class ParsingFailedException : Exception
    {
        public string Input { get; set; }
        
        public ParsingFailedException(string input)
        {
            Input = input;
        }
    }
}