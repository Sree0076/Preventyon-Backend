using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Preventyon.Migrations
{
    /// <inheritdoc />
    public partial class table1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Permission_permissionID",
                table: "Role");

            migrationBuilder.RenameColumn(
                name: "permissionID",
                table: "Role",
                newName: "PermissionID");

            migrationBuilder.RenameIndex(
                name: "IX_Role_permissionID",
                table: "Role",
                newName: "IX_Role_PermissionID");

            migrationBuilder.RenameColumn(
                name: "userManagement",
                table: "Permission",
                newName: "UserManagement");

            migrationBuilder.RenameColumn(
                name: "permissionName",
                table: "Permission",
                newName: "PermissionName");

            migrationBuilder.RenameColumn(
                name: "incidentManagement",
                table: "Permission",
                newName: "IncidentManagement");

            migrationBuilder.RenameColumn(
                name: "incidentCreateOnly",
                table: "Permission",
                newName: "IncidentCreateOnly");

            migrationBuilder.RenameColumn(
                name: "roleOfReporter",
                table: "Incident",
                newName: "RoleOfReporter");

            migrationBuilder.RenameColumn(
                name: "reportedBy",
                table: "Incident",
                newName: "ReportedBy");

            migrationBuilder.RenameColumn(
                name: "priority",
                table: "Incident",
                newName: "Priority");

            migrationBuilder.RenameColumn(
                name: "monthYear",
                table: "Incident",
                newName: "MonthYear");

            migrationBuilder.RenameColumn(
                name: "isDraft",
                table: "Incident",
                newName: "IsDraft");

            migrationBuilder.RenameColumn(
                name: "investigationDetails",
                table: "Incident",
                newName: "InvestigationDetails");

            migrationBuilder.RenameColumn(
                name: "incidentType",
                table: "Incident",
                newName: "IncidentType");

            migrationBuilder.RenameColumn(
                name: "incidentTitle",
                table: "Incident",
                newName: "IncidentTitle");

            migrationBuilder.RenameColumn(
                name: "incidentStatus",
                table: "Incident",
                newName: "IncidentStatus");

            migrationBuilder.RenameColumn(
                name: "incidentOccuredDate",
                table: "Incident",
                newName: "IncidentOccuredDate");

            migrationBuilder.RenameColumn(
                name: "incidentNo",
                table: "Incident",
                newName: "IncidentNo");

            migrationBuilder.RenameColumn(
                name: "incidentDescription",
                table: "Incident",
                newName: "IncidentDescription");

            migrationBuilder.RenameColumn(
                name: "deptOfAssignee",
                table: "Incident",
                newName: "DeptOfAssignee");

            migrationBuilder.RenameColumn(
                name: "correctiveDetailsTimeTakenToCloseIncident",
                table: "Incident",
                newName: "CorrectiveDetailsTimeTakenToCloseIncident");

            migrationBuilder.RenameColumn(
                name: "correctiveActualCompletionDate",
                table: "Incident",
                newName: "CorrectiveActualCompletionDate");

            migrationBuilder.RenameColumn(
                name: "correctiveAction",
                table: "Incident",
                newName: "CorrectiveAction");

            migrationBuilder.RenameColumn(
                name: "correctionDetailsTimeTakenToCloseIncident",
                table: "Incident",
                newName: "CorrectionDetailsTimeTakenToCloseIncident");

            migrationBuilder.RenameColumn(
                name: "correctionCompletionTargetDate",
                table: "Incident",
                newName: "CorrectionCompletionTargetDate");

            migrationBuilder.RenameColumn(
                name: "correctionActualCompletionDate",
                table: "Incident",
                newName: "CorrectionActualCompletionDate");

            migrationBuilder.RenameColumn(
                name: "correction",
                table: "Incident",
                newName: "Correction");

            migrationBuilder.RenameColumn(
                name: "collectionOfEvidence",
                table: "Incident",
                newName: "CollectionOfEvidence");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Incident",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "associatedImpacts",
                table: "Incident",
                newName: "AssociatedImpacts");

            migrationBuilder.RenameColumn(
                name: "actionAssignedTo",
                table: "Incident",
                newName: "ActionAssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Permission_PermissionID",
                table: "Role",
                column: "PermissionID",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Role_Permission_PermissionID",
                table: "Role");

            migrationBuilder.RenameColumn(
                name: "PermissionID",
                table: "Role",
                newName: "permissionID");

            migrationBuilder.RenameIndex(
                name: "IX_Role_PermissionID",
                table: "Role",
                newName: "IX_Role_permissionID");

            migrationBuilder.RenameColumn(
                name: "UserManagement",
                table: "Permission",
                newName: "userManagement");

            migrationBuilder.RenameColumn(
                name: "PermissionName",
                table: "Permission",
                newName: "permissionName");

            migrationBuilder.RenameColumn(
                name: "IncidentManagement",
                table: "Permission",
                newName: "incidentManagement");

            migrationBuilder.RenameColumn(
                name: "IncidentCreateOnly",
                table: "Permission",
                newName: "incidentCreateOnly");

            migrationBuilder.RenameColumn(
                name: "RoleOfReporter",
                table: "Incident",
                newName: "roleOfReporter");

            migrationBuilder.RenameColumn(
                name: "ReportedBy",
                table: "Incident",
                newName: "reportedBy");

            migrationBuilder.RenameColumn(
                name: "Priority",
                table: "Incident",
                newName: "priority");

            migrationBuilder.RenameColumn(
                name: "MonthYear",
                table: "Incident",
                newName: "monthYear");

            migrationBuilder.RenameColumn(
                name: "IsDraft",
                table: "Incident",
                newName: "isDraft");

            migrationBuilder.RenameColumn(
                name: "InvestigationDetails",
                table: "Incident",
                newName: "investigationDetails");

            migrationBuilder.RenameColumn(
                name: "IncidentType",
                table: "Incident",
                newName: "incidentType");

            migrationBuilder.RenameColumn(
                name: "IncidentTitle",
                table: "Incident",
                newName: "incidentTitle");

            migrationBuilder.RenameColumn(
                name: "IncidentStatus",
                table: "Incident",
                newName: "incidentStatus");

            migrationBuilder.RenameColumn(
                name: "IncidentOccuredDate",
                table: "Incident",
                newName: "incidentOccuredDate");

            migrationBuilder.RenameColumn(
                name: "IncidentNo",
                table: "Incident",
                newName: "incidentNo");

            migrationBuilder.RenameColumn(
                name: "IncidentDescription",
                table: "Incident",
                newName: "incidentDescription");

            migrationBuilder.RenameColumn(
                name: "DeptOfAssignee",
                table: "Incident",
                newName: "deptOfAssignee");

            migrationBuilder.RenameColumn(
                name: "CorrectiveDetailsTimeTakenToCloseIncident",
                table: "Incident",
                newName: "correctiveDetailsTimeTakenToCloseIncident");

            migrationBuilder.RenameColumn(
                name: "CorrectiveActualCompletionDate",
                table: "Incident",
                newName: "correctiveActualCompletionDate");

            migrationBuilder.RenameColumn(
                name: "CorrectiveAction",
                table: "Incident",
                newName: "correctiveAction");

            migrationBuilder.RenameColumn(
                name: "CorrectionDetailsTimeTakenToCloseIncident",
                table: "Incident",
                newName: "correctionDetailsTimeTakenToCloseIncident");

            migrationBuilder.RenameColumn(
                name: "CorrectionCompletionTargetDate",
                table: "Incident",
                newName: "correctionCompletionTargetDate");

            migrationBuilder.RenameColumn(
                name: "CorrectionActualCompletionDate",
                table: "Incident",
                newName: "correctionActualCompletionDate");

            migrationBuilder.RenameColumn(
                name: "Correction",
                table: "Incident",
                newName: "correction");

            migrationBuilder.RenameColumn(
                name: "CollectionOfEvidence",
                table: "Incident",
                newName: "collectionOfEvidence");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Incident",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "AssociatedImpacts",
                table: "Incident",
                newName: "associatedImpacts");

            migrationBuilder.RenameColumn(
                name: "ActionAssignedTo",
                table: "Incident",
                newName: "actionAssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Permission_permissionID",
                table: "Role",
                column: "permissionID",
                principalTable: "Permission",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
