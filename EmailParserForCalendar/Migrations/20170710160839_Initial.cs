using System;
using System.Collections.Generic;
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
                    GoodleId = table.Column<string>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EventDate = table.Column<DateTimeOffset>(nullable: false),
                    From = table.Column<string>(nullable: true),
                    MiscInformation = table.Column<string>(nullable: true),
                    People = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.GoodleId);
                });

            migrationBuilder.CreateTable(
                name: "ForwardedEmails",
                columns: table => new
                {
                    GoodleId = table.Column<string>(nullable: false),
                    CalendarEventGoodleId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    EmailDate = table.Column<DateTime>(nullable: true),
                    Operation = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    Subject = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForwardedEmails", x => x.GoodleId);
                    table.ForeignKey(
                        name: "FK_ForwardedEmails_CalendarEvents_CalendarEventGoodleId",
                        column: x => x.CalendarEventGoodleId,
                        principalTable: "CalendarEvents",
                        principalColumn: "GoodleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForwardedEmails_CalendarEventGoodleId",
                table: "ForwardedEmails",
                column: "CalendarEventGoodleId");
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
