using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnitcherPortal.Migrations
{
    /// <inheritdoc />
    public partial class Update3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppSupervisedComputers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppSupervisedComputers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "AppSupervisedComputers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "AppSupervisedComputers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AppSupervisedComputers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppSupervisedComputers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppSupervisedComputers",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppSupervisedComputers");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppSupervisedComputers");
        }
    }
}
