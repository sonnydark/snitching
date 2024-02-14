using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnitcherPortal.Migrations
{
    /// <inheritdoc />
    public partial class Update4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DetectedProcesses",
                table: "AppActivityRecords",
                newName: "Data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "AppActivityRecords",
                newName: "DetectedProcesses");
        }
    }
}
