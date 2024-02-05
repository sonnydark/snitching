using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnitcherPortal.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSupervisedComputers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Identifier = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IsCalendarActive = table.Column<bool>(type: "bit", nullable: false),
                    BanUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSupervisedComputers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppActivityRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisedComputerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DetectedProcesses = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppActivityRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppActivityRecords_AppSupervisedComputers_SupervisedComputerId",
                        column: x => x.SupervisedComputerId,
                        principalTable: "AppSupervisedComputers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppCalendars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisedComputerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    AllowedHours = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCalendars_AppSupervisedComputers_SupervisedComputerId",
                        column: x => x.SupervisedComputerId,
                        principalTable: "AppSupervisedComputers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppKnownProcesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisedComputerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    IsHidden = table.Column<bool>(type: "bit", nullable: false),
                    IsImportant = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppKnownProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppKnownProcesses_AppSupervisedComputers_SupervisedComputerId",
                        column: x => x.SupervisedComputerId,
                        principalTable: "AppSupervisedComputers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppSnitchingLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisedComputerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSnitchingLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSnitchingLogs_AppSupervisedComputers_SupervisedComputerId",
                        column: x => x.SupervisedComputerId,
                        principalTable: "AppSupervisedComputers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppActivityRecords_SupervisedComputerId",
                table: "AppActivityRecords",
                column: "SupervisedComputerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCalendars_SupervisedComputerId",
                table: "AppCalendars",
                column: "SupervisedComputerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppKnownProcesses_SupervisedComputerId",
                table: "AppKnownProcesses",
                column: "SupervisedComputerId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSnitchingLogs_SupervisedComputerId",
                table: "AppSnitchingLogs",
                column: "SupervisedComputerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppActivityRecords");

            migrationBuilder.DropTable(
                name: "AppCalendars");

            migrationBuilder.DropTable(
                name: "AppKnownProcesses");

            migrationBuilder.DropTable(
                name: "AppSnitchingLogs");

            migrationBuilder.DropTable(
                name: "AppSupervisedComputers");
        }
    }
}
