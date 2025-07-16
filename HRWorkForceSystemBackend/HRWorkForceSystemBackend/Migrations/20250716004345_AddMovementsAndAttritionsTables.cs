using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HRWorkForceSystemBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddMovementsAndAttritionsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attritions_Employees_EmployeeID",
                table: "Attritions");

            migrationBuilder.DropForeignKey(
                name: "FK_Movements_Employees_EmployeeID",
                table: "Movements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movements",
                table: "Movements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attritions",
                table: "Attritions");

            migrationBuilder.RenameTable(
                name: "Movements",
                newName: "Movement");

            migrationBuilder.RenameTable(
                name: "Attritions",
                newName: "Attrition");

            migrationBuilder.RenameIndex(
                name: "IX_Movements_EmployeeID",
                table: "Movement",
                newName: "IX_Movement_EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Attritions_EmployeeID",
                table: "Attrition",
                newName: "IX_Attrition_EmployeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movement",
                table: "Movement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attrition",
                table: "Attrition",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attrition_Employees_EmployeeID",
                table: "Attrition",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movement_Employees_EmployeeID",
                table: "Movement",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attrition_Employees_EmployeeID",
                table: "Attrition");

            migrationBuilder.DropForeignKey(
                name: "FK_Movement_Employees_EmployeeID",
                table: "Movement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Movement",
                table: "Movement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Attrition",
                table: "Attrition");

            migrationBuilder.RenameTable(
                name: "Movement",
                newName: "Movements");

            migrationBuilder.RenameTable(
                name: "Attrition",
                newName: "Attritions");

            migrationBuilder.RenameIndex(
                name: "IX_Movement_EmployeeID",
                table: "Movements",
                newName: "IX_Movements_EmployeeID");

            migrationBuilder.RenameIndex(
                name: "IX_Attrition_EmployeeID",
                table: "Attritions",
                newName: "IX_Attritions_EmployeeID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Movements",
                table: "Movements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Attritions",
                table: "Attritions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attritions_Employees_EmployeeID",
                table: "Attritions",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Movements_Employees_EmployeeID",
                table: "Movements",
                column: "EmployeeID",
                principalTable: "Employees",
                principalColumn: "EmployeeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
