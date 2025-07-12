using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class FinalInitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attritions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attritions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HRUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SkillName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Summaries",
                columns: table => new
                {
                    TotalPromotions = table.Column<int>(type: "int", nullable: false),
                    TotalExits = table.Column<int>(type: "int", nullable: false),
                    TotalTransfers = table.Column<int>(type: "int", nullable: false),
                    TotalAttritions = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "TrainingPrograms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Availability = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingPrograms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkforceUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentRole = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforceUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    ProficiencyLevel = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_HRUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "HRUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeSkills_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSkillRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    SkillId = table.Column<int>(type: "int", nullable: false),
                    RequiredProficiencyLevel = table.Column<int>(type: "int", nullable: false),
                    NumberOfResourcesNeeded = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSkillRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSkillRequirements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSkillRequirements_Skills_SkillId",
                        column: x => x.SkillId,
                        principalTable: "Skills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainingProgramId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_TrainingPrograms_TrainingProgramId",
                        column: x => x.TrainingProgramId,
                        principalTable: "TrainingPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_EmployeeId",
                table: "EmployeeSkills",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSkills_SkillId",
                table: "EmployeeSkills",
                column: "SkillId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_TrainingProgramId",
                table: "Enrollments",
                column: "TrainingProgramId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkillRequirements_ProjectId",
                table: "ProjectSkillRequirements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSkillRequirements_SkillId",
                table: "ProjectSkillRequirements",
                column: "SkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Attritions");

            migrationBuilder.DropTable(
                name: "EmployeeSkills");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Movements");

            migrationBuilder.DropTable(
                name: "ProjectSkillRequirements");

            migrationBuilder.DropTable(
                name: "Summaries");

            migrationBuilder.DropTable(
                name: "WorkforceUsers");

            migrationBuilder.DropTable(
                name: "HRUsers");

            migrationBuilder.DropTable(
                name: "TrainingPrograms");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Skills");
        }
    }
}
