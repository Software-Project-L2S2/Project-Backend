using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrationAddHRProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HRProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HRId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Contact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accounts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HRUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HRProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HRProfiles_HRUsers_HRUserId",
                        column: x => x.HRUserId,
                        principalTable: "HRUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HRProfiles_HRUserId",
                table: "HRProfiles",
                column: "HRUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HRProfiles");
        }
    }
}
