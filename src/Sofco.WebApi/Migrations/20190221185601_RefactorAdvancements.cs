using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAdvancements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                schema: "app",
                table: "WorkTimes",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Version",
                schema: "app",
                table: "Workflows",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowId",
                schema: "app",
                table: "Advancements",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_WorkflowId",
                schema: "app",
                table: "Advancements",
                column: "WorkflowId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Workflows_WorkflowId",
                schema: "app",
                table: "Advancements",
                column: "WorkflowId",
                principalSchema: "app",
                principalTable: "Workflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Workflows_WorkflowId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropIndex(
                name: "IX_Advancements_WorkflowId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "WorkflowId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.AlterColumn<string>(
                name: "Reference",
                schema: "app",
                table: "WorkTimes",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Version",
                schema: "app",
                table: "Workflows",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
