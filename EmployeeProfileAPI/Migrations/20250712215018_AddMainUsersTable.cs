using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProfileAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddMainUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MainUserId",
                table: "EmployeeProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MainUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_MainUserId",
                table: "EmployeeProfiles",
                column: "MainUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProfiles_MainUsers_MainUserId",
                table: "EmployeeProfiles",
                column: "MainUserId",
                principalTable: "MainUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfiles_MainUsers_MainUserId",
                table: "EmployeeProfiles");

            migrationBuilder.DropTable(
                name: "MainUsers");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProfiles_MainUserId",
                table: "EmployeeProfiles");

            migrationBuilder.DropColumn(
                name: "MainUserId",
                table: "EmployeeProfiles");
        }
    }
}
