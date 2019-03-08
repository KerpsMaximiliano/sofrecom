using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorRefunds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkflowId",
                schema: "app",
                table: "Refunds",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_WorkflowId",
                schema: "app",
                table: "Refunds",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Workflows_WorkflowId",
                schema: "app",
                table: "Refunds",
                column: "WorkflowId",
                principalSchema: "app",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Workflows_WorkflowId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_WorkflowId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                schema: "app",
                table: "Refunds");
        }
    }
}
