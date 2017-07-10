using System.Text.RegularExpressions;
using EmailParserForCalendar.Exceptions;

namespace EmailParserForCalendar
{
    public class SujectLineParser
    {
        private const string InvitationAndReschedulePattern =
            @"Forwarded meeting Notice from (.+?): (.+?): ([^\(]*)(\s*(\(.*\))?\s*\((.+(PM|AM))\s*(ZE5B|CEDT|EDT)\s*(.*)?)?";

        private const string ReplyPattern = @"Forwarded meeting Reply from (.+?): (.+?): (.*)";

        private const string UpdatePattern =
            @"Forwarded meeting Notice from (.+?): Information Update - (\w+) has changed: ([^\(]+)\s*(\(.*\))?";

        private const string CancelledPattern = @"Forwarded meeting Notice from (.*?): (Cancelled): (.*)";

        private static readonly Regex InvitationAndRescheduleRegex = new Regex(InvitationAndReschedulePattern, RegexOptions.Compiled);

        private static readonly Regex ReplyRegex = new Regex(ReplyPattern, RegexOptions.Compiled);

        private static readonly Regex UpdateRegex = new Regex(UpdatePattern, RegexOptions.Compiled);
        
        private static readonly Regex CancelledRegex = new Regex(CancelledPattern, RegexOptions.Compiled);
        
        public static Match Parse(string input)
        {
            Regex[] regexExpressions = {InvitationAndRescheduleRegex, ReplyRegex, UpdateRegex, CancelledRegex};

            foreach (var regexExpression in regexExpressions)
            {
                Match match = regexExpression.Match(input);
                if (match.Success) return match;
            }

            throw new ParsingFailedException(input);
        }
    }
}