using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GuestRegistration.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    AdditionalInformation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discriminator = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: true),
                    RegisterCode = table.Column<string>(type: "TEXT", nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    IdCode = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipants",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "INTEGER", nullable: false),
                    ParticipantId = table.Column<long>(type: "INTEGER", nullable: false),
                    PaymentMethodId = table.Column<long>(type: "INTEGER", nullable: false),
                    NumberOfAttendees = table.Column<int>(type: "INTEGER", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipants", x => new { x.EventId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_EventParticipants_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipants_Participants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventParticipants_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AdditionalInformation", "Location", "Name", "StartTime" },
                values: new object[,]
                {
                    { 1L, null, "Tech Park Tallinn", "Summer Tech Conference", new DateTime(2025, 9, 16, 8, 31, 2, 365, DateTimeKind.Utc).AddTicks(8845) },
                    { 2L, null, "Virtual Event", "Agile Development Workshop", new DateTime(2025, 11, 15, 8, 31, 2, 365, DateTimeKind.Utc).AddTicks(9250) },
                    { 3L, null, "Pärnu Hotel", "Winter Code Retreat", new DateTime(2025, 6, 18, 8, 31, 2, 365, DateTimeKind.Utc).AddTicks(9254) },
                    { 4L, null, "Tartu University", "Project Management Meetup", new DateTime(2025, 4, 19, 8, 31, 2, 365, DateTimeKind.Utc).AddTicks(9255) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_ParticipantId",
                table: "EventParticipants",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipants_PaymentMethodId",
                table: "EventParticipants",
                column: "PaymentMethodId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventParticipants");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "PaymentMethods");
        }
    }
}
