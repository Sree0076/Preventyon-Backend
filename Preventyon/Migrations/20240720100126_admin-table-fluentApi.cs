using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Preventyon.Migrations
{
    /// <inheritdoc />
    public partial class admintablefluentApi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Employees_EmployeeId",
                table: "Admins");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Employees_EmployeeId",
                table: "Admins",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_Employees_EmployeeId",
                table: "Admins");

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_Employees_EmployeeId",
                table: "Admins",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
