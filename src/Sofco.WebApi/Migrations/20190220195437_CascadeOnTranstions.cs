using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CascadeOnTranstions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStateAccesses_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStateNotifiers_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateNotifiers");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStateAccesses_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "WorkflowStateTransitionId",
                principalSchema: "app",
                principalTable: "WorkflowStateTransitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStateNotifiers_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "WorkflowStateTransitionId",
                principalSchema: "app",
                principalTable: "WorkflowStateTransitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStateAccesses_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStateNotifiers_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateNotifiers");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStateAccesses_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "WorkflowStateTransitionId",
                principalSchema: "app",
                principalTable: "WorkflowStateTransitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStateNotifiers_WorkflowStateTransitions_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "WorkflowStateTransitionId",
                principalSchema: "app",
                principalTable: "WorkflowStateTransitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
