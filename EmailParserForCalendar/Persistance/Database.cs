using Microsoft.EntityFrameworkCore;

namespace EmailParserForCalendar.Persistance
{
    public class Database : DbContext
    {
        public DbSet<ForwardedEmail> ForwardedEmails { get; set; }
        
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=emailParser.db");
        }
    }
}