using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProfileAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeProfileUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
              //  name: "Role",
              //  table: "WorkforceUsers");

           // migrationBuilder.DropColumn(
             //   name: "Role",
              //  table: "HRUsers");

          //  migrationBuilder.DropColumn(
              //  name: "AuthUserId",
              //  table: "Employees");

          //  migrationBuilder.DropColumn(
               // name: "Role",
               // table: "Admins");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogo",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeProfiles",
                columns: table => new
                {
                    EmployeeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Designation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProfileImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyLogo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfiles", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_EmployeeProfiles_SystemUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "SystemUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfileEducation",
                columns: table => new
                {
                    EducationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qualification = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileEducation", x => x.EducationID);
                    table.ForeignKey(
                        name: "FK_ProfileEducation_EmployeeProfiles_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeProfiles",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSkills",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Level = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSkills", x => x.SkillID);
                    table.ForeignKey(
                        name: "FK_ProfileSkills_EmployeeProfiles_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "EmployeeProfiles",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProfiles_UserID",
                table: "EmployeeProfiles",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileEducation_EmployeeID",
                table: "ProfileEducation",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSkills_EmployeeID",
                table: "ProfileSkills",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProfileEducation");

            migrationBuilder.DropTable(
                name: "ProfileSkills");

            migrationBuilder.DropTable(
                name: "EmployeeProfiles");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "WorkforceUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "HRUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogo",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "AuthUserId",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
