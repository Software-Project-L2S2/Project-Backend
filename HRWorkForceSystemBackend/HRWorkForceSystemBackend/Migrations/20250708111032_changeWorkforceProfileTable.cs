using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class changeWorkforceProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentRole",
                table: "WorkforceProfiles",
                newName: "Name");

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "WorkforceProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "WorkforceProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobCategory",
                table: "WorkforceProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "WorkforceProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "WorkforceProfiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "WorkforceProfiles");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "WorkforceProfiles");

            migrationBuilder.DropColumn(
                name: "JobCategory",
                table: "WorkforceProfiles");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "WorkforceProfiles");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "WorkforceProfiles");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "WorkforceProfiles",
                newName: "CurrentRole");
        }
    }
}
