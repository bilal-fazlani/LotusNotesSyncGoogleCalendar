namespace EmailParserForCalendar
{
    public class Settings
    {
        public string From { get; set; }
        public int BatchSize { get; set; }
        public int FetchIntervalInHours { get; set; }
        public int MaxNumerOfDaysToFetchEmail { get; set; }
    }
}