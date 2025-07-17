using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStatusFromMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Movements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Movements",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
