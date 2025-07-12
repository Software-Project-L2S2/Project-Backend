using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkforceDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentRole",
                table: "WorkforceUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "WorkforceUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkforceId",
                table: "WorkforceUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "HRUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HrId",
                table: "HRUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentRole",
                table: "WorkforceUsers");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "WorkforceUsers");

            migrationBuilder.DropColumn(
                name: "WorkforceId",
                table: "WorkforceUsers");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "HRUsers");

            migrationBuilder.DropColumn(
                name: "HrId",
                table: "HRUsers");
        }
    }
}
