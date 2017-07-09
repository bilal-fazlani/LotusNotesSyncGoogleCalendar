using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using EmailParserForCalendar.Persistance;

namespace EmailParserForCalendar.Migrations
{
    [DbContext(typeof(Database))]
    partial class DatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("EmailParserForCalendar.Persistance.CalendarEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("EventDate");

                    b.Property<string>("From");

                    b.Property<string>("GoodleId");

                    b.Property<string>("MiscInformation");

                    b.Property<string>("People");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("CalendarEvents");
                });

            modelBuilder.Entity("EmailParserForCalendar.Persistance.ForwardedEmail", b =>
                {
                    b.Property<string>("GoodleId")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("CalendarEventId");

                    b.Property<string>("Operation");

                    b.Property<string>("Status");

                    b.Property<string>("Subject");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("GoodleId");

                    b.HasIndex("CalendarEventId");

                    b.ToTable("ForwardedEmails");
                });

            modelBuilder.Entity("EmailParserForCalendar.Persistance.ForwardedEmail", b =>
                {
                    b.HasOne("EmailParserForCalendar.Persistance.CalendarEvent")
                        .WithMany("RelatedEmails")
                        .HasForeignKey("CalendarEventId");
                });
        }
    }
}
