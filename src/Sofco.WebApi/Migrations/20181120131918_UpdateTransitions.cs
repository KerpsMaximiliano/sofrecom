using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateTransitions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConditionCode",
                schema: "app",
                table: "WorkflowStateTransitions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                schema: "app",
                table: "Advancements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_StatusId",
                schema: "app",
                table: "Advancements",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_WorkflowStates_StatusId",
                schema: "app",
                table: "Advancements",
                column: "StatusId",
                principalSchema: "app",
                principalTable: "WorkflowStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_WorkflowStates_StatusId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropIndex(
                name: "IX_Advancements_StatusId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "ConditionCode",
                schema: "app",
                table: "WorkflowStateTransitions");

            migrationBuilder.DropColumn(
                name: "StatusId",
                schema: "app",
                table: "Advancements");
        }
    }
}
