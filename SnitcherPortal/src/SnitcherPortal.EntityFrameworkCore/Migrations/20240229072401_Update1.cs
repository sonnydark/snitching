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
            migrationBuilder.AddColumn<int>(
                name: "ClientHeartbeat",
                table: "AppSupervisedComputers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "EnableAutokillReasoning",
                table: "AppSupervisedComputers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientHeartbeat",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "EnableAutokillReasoning",
                table: "AppSupervisedComputers");
        }
    }
}
