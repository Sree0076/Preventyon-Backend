using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Preventyon.Migrations
{
    /// <inheritdoc />
    public partial class table4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    permissionName = table.Column<string>(type: "text", nullable: true),
                    incidentManagement = table.Column<bool>(type: "boolean", nullable: false),
                    userManagement = table.Column<bool>(type: "boolean", nullable: false),
                    incidentCreateOnly = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    permissionID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_Permission_permissionID",
                        column: x => x.permissionID,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleId = table.Column<int>(type: "integer", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employees_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Incident",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    incidentNo = table.Column<string>(type: "text", nullable: true),
                    incidentTitle = table.Column<string>(type: "text", nullable: true),
                    incidentDescription = table.Column<string>(type: "text", nullable: true),
                    reportedBy = table.Column<string>(type: "text", nullable: true),
                    roleOfReporter = table.Column<string>(type: "text", nullable: true),
                    incidentOccuredDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    monthYear = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    incidentType = table.Column<string>(type: "text", nullable: true),
                    category = table.Column<string>(type: "text", nullable: true),
                    priority = table.Column<string>(type: "text", nullable: true),
                    actionAssignedTo = table.Column<string>(type: "text", nullable: true),
                    deptOfAssignee = table.Column<string>(type: "text", nullable: true),
                    investigationDetails = table.Column<string>(type: "text", nullable: true),
                    associatedImpacts = table.Column<string>(type: "text", nullable: true),
                    collectionOfEvidence = table.Column<string>(type: "text", nullable: true),
                    correction = table.Column<string>(type: "text", nullable: true),
                    correctiveAction = table.Column<string>(type: "text", nullable: true),
                    correctionCompletionTargetDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    correctionActualCompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    correctiveActualCompletionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    incidentStatus = table.Column<string>(type: "text", nullable: true),
                    correctionDetailsTimeTakenToCloseIncident = table.Column<decimal>(type: "numeric", nullable: false),
                    correctiveDetailsTimeTakenToCloseIncident = table.Column<decimal>(type: "numeric", nullable: false),
                    isDraft = table.Column<bool>(type: "boolean", nullable: false),
                    EmployeeId = table.Column<int>(type: "integer", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Incident", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Incident_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_RoleId",
                table: "Employees",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Incident_EmployeeId",
                table: "Incident",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_permissionID",
                table: "Role",
                column: "permissionID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Incident");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Permission");
        }
    }
}
