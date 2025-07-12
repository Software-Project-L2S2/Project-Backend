using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkforceProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CurrentRole",
            //    table: "WorkforceUsers");

            //migrationBuilder.DropColumn(
            //    name: "Department",
            //    table: "WorkforceUsers");

            //migrationBuilder.DropColumn(
            //    name: "WorkforceId",
            //    table: "WorkforceUsers");

            //migrationBuilder.DropColumn(
            //    name: "Department",
            //    table: "HRUsers");

            //migrationBuilder.DropColumn(
            //    name: "HrId",
            //    table: "HRUsers");

            migrationBuilder.CreateTable(
                name: "WorkforceProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkforceId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrentRole = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkforceUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkforceProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkforceProfiles_WorkforceUsers_WorkforceUserId",
                        column: x => x.WorkforceUserId,
                        principalTable: "WorkforceUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkforceProfiles_WorkforceUserId",
                table: "WorkforceProfiles",
                column: "WorkforceUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkforceProfiles");

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
    }
}
