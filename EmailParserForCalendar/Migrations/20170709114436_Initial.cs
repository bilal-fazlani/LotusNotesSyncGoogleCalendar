using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EmailParserForCalendar.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventDate = table.Column<DateTimeOffset>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    GoodleId = table.Column<string>(nullable: true),
                    MiscInformation = table.Column<string>(nullable: true),
                    People = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForwardedEmails",
                columns: table => new
                {
                    GoodleId = table.Column<string>(nullable: false),
                    CalendarEventId = table.Column<long>(nullable: true),
                    Operation = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    TimeStamp = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForwardedEmails", x => x.GoodleId);
                    table.ForeignKey(
                        name: "FK_ForwardedEmails_CalendarEvents_CalendarEventId",
                        column: x => x.CalendarEventId,
                        principalTable: "CalendarEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForwardedEmails_CalendarEventId",
                table: "ForwardedEmails",
                column: "CalendarEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForwardedEmails");

            migrationBuilder.DropTable(
                name: "CalendarEvents");
        }
    }
}
