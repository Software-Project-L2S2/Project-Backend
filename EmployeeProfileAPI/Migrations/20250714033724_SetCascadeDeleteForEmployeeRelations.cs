using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeProfileAPI.Migrations
{
    /// <inheritdoc />
    public partial class SetCascadeDeleteForEmployeeRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfiles_MainUsers_MainUserId",
                table: "EmployeeProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfiles_SystemUsers_UserID",
                table: "EmployeeProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileEducation_EmployeeProfiles_EmployeeID",
                table: "ProfileEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSkills_EmployeeProfiles_EmployeeID",
                table: "ProfileSkills");

            migrationBuilder.DropTable(
                name: "MainUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemUsers",
                table: "SystemUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileSkills",
                table: "ProfileSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeProfiles",
                table: "EmployeeProfiles");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeProfiles_MainUserId",
                table: "EmployeeProfiles");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "SystemUsers");

            migrationBuilder.DropColumn(
                name: "MainUserId",
                table: "EmployeeProfiles");

            migrationBuilder.RenameTable(
                name: "SystemUsers",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "ProfileSkills",
                newName: "ProfileSkill");

            migrationBuilder.RenameTable(
                name: "EmployeeProfiles",
                newName: "EmployeeProfile");

            migrationBuilder.RenameIndex(
                name: "IX_ProfileSkills_EmployeeID",
                table: "ProfileSkill",
                newName: "IX_ProfileSkill_EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeProfiles_UserID",
                table: "EmployeeProfile",
                newName: "IX_EmployeeProfile_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Qualification",
                table: "ProfileEducation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogo",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "SkillName",
                table: "ProfileSkill",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProfileSkill",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "EmployeeProfile",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Designation",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Contact",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogo",
                table: "EmployeeProfile",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileSkill",
                table: "ProfileSkill",
                column: "SkillID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeProfile",
                table: "EmployeeProfile",
                column: "EmployeeID");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProfile_Users_UserID",
                table: "EmployeeProfile",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileEducation_EmployeeProfile_EmployeeID",
                table: "ProfileEducation",
                column: "EmployeeID",
                principalTable: "EmployeeProfile",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSkill_EmployeeProfile_EmployeeID",
                table: "ProfileSkill",
                column: "EmployeeID",
                principalTable: "EmployeeProfile",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeProfile_Users_UserID",
                table: "EmployeeProfile");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileEducation_EmployeeProfile_EmployeeID",
                table: "ProfileEducation");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileSkill_EmployeeProfile_EmployeeID",
                table: "ProfileSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProfileSkill",
                table: "ProfileSkill");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeProfile",
                table: "EmployeeProfile");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "SystemUsers");

            migrationBuilder.RenameTable(
                name: "ProfileSkill",
                newName: "ProfileSkills");

            migrationBuilder.RenameTable(
                name: "EmployeeProfile",
                newName: "EmployeeProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_ProfileSkill_EmployeeID",
                table: "ProfileSkills",
                newName: "IX_ProfileSkills_EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeProfile_UserID",
                table: "EmployeeProfiles",
                newName: "IX_EmployeeProfiles_UserID");

            migrationBuilder.AlterColumn<string>(
                name: "Qualification",
                table: "ProfileEducation",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogo",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "PasswordHash",
                table: "SystemUsers",
                type: "varbinary(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SystemUsers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "SystemUsers",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "SkillName",
                table: "ProfileSkills",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "ProfileSkills",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "EmployeeProfiles",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ProfileImage",
                table: "EmployeeProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EmployeeProfiles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "EmployeeProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EmployeeProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Designation",
                table: "EmployeeProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Department",
                table: "EmployeeProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Contact",
                table: "EmployeeProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyLogo",
                table: "EmployeeProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "MainUserId",
                table: "EmployeeProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemUsers",
                table: "SystemUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProfileSkills",
                table: "ProfileSkills",
                column: "SkillID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeProfiles",
                table: "EmployeeProfiles",
                column: "EmployeeID");

            migrationBuilder.CreateTable(
                name: "MainUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
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

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeProfiles_SystemUsers_UserID",
                table: "EmployeeProfiles",
                column: "UserID",
                principalTable: "SystemUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileEducation_EmployeeProfiles_EmployeeID",
                table: "ProfileEducation",
                column: "EmployeeID",
                principalTable: "EmployeeProfiles",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileSkills_EmployeeProfiles_EmployeeID",
                table: "ProfileSkills",
                column: "EmployeeID",
                principalTable: "EmployeeProfiles",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
