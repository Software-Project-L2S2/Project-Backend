using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmployeeSkills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmployeeSkills",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmployeeSkills",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmployeeSkills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmployeeSkills",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ProjectSkillRequirements",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ProjectSkillRequirements",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ProjectSkillRequirements",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ProjectSkillRequirements",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "HRUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HRUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HRUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Projects",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Skills",
                keyColumn: "Id",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HRUsers",
                columns: new[] { "Id", "Department", "Email", "FirstName", "FullName", "LastName", "PasswordHash", "PhoneNumber", "Role", "UserName" },
                values: new object[,]
                {
                    { 1, "Engineering", "john.doe@example.com", "John", "John Doe", "Doe", "hashed_password", "", "Senior Developer", "john.doe@example.com" },
                    { 2, "Engineering", "jane.smith@example.com", "Jane", "Jane Smith", "Smith", "hashed_password", "", "Frontend Developer", "jane.smith@example.com" },
                    { 3, "Quality Assurance", "peter.jones@example.com", "Peter", "Peter Jones", "Jones", "hashed_password", "", "QA Engineer", "peter.jones@example.com" }
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "Description", "EndDate", "IsActive", "ProjectName", "StartDate" },
                values: new object[,]
                {
                    { 1, "Migrating legacy backend to .NET Core.", null, true, "E-commerce Backend Migration", new DateTime(2025, 7, 10, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9127) },
                    { 2, "Building a new SPA using React.", null, true, "New Customer Portal (React)", new DateTime(2025, 8, 9, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9131) }
                });

            migrationBuilder.InsertData(
                table: "Skills",
                columns: new[] { "Id", "SkillName" },
                values: new object[,]
                {
                    { 1, "C#" },
                    { 2, ".NET Core" },
                    { 3, "React" },
                    { 4, "SQL Server" },
                    { 5, "Azure DevOps" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeSkills",
                columns: new[] { "Id", "EmployeeId", "LastUpdated", "ProficiencyLevel", "SkillId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 10, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9173), 5, 1 },
                    { 2, 1, new DateTime(2025, 7, 10, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9174), 5, 2 },
                    { 3, 1, new DateTime(2025, 7, 10, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9175), 4, 4 },
                    { 4, 2, new DateTime(2025, 7, 10, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9177), 5, 3 },
                    { 5, 3, new DateTime(2025, 7, 10, 8, 25, 40, 788, DateTimeKind.Utc).AddTicks(9178), 3, 5 }
                });

            migrationBuilder.InsertData(
                table: "ProjectSkillRequirements",
                columns: new[] { "Id", "NumberOfResourcesNeeded", "ProjectId", "RequiredProficiencyLevel", "SkillId" },
                values: new object[,]
                {
                    { 1, 2, 1, 4, 1 },
                    { 2, 2, 1, 4, 2 },
                    { 3, 1, 1, 3, 4 },
                    { 4, 3, 2, 4, 3 }
                });
        }
    }
}
