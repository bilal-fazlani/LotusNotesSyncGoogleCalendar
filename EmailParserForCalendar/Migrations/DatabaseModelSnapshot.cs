using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
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
                    b.Property<string>("GoodleId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<DateTimeOffset>("EventDate");

                    b.Property<string>("From");

                    b.Property<string>("MiscInformation");

                    b.Property<string>("People");

                    b.Property<string>("Title");

                    b.HasKey("GoodleId");

                    b.ToTable("CalendarEvents");
                });

            modelBuilder.Entity("EmailParserForCalendar.Persistance.ForwardedEmail", b =>
                {
                    b.Property<string>("GoodleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CalendarEventGoodleId");

                    b.Property<string>("Operation");

                    b.Property<string>("Status");

                    b.Property<string>("Subject");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<string>("Title");

                    b.HasKey("GoodleId");

                    b.HasIndex("CalendarEventGoodleId");

                    b.ToTable("ForwardedEmails");
                });

            modelBuilder.Entity("EmailParserForCalendar.Persistance.ForwardedEmail", b =>
                {
                    b.HasOne("EmailParserForCalendar.Persistance.CalendarEvent")
                        .WithMany("RelatedEmails")
                        .HasForeignKey("CalendarEventGoodleId");
                });
        }
    }
}
