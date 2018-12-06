using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeAdvancement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActionName",
                schema: "app",
                table: "WorkflowStates",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "app",
                table: "Advancements",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 400,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionName",
                schema: "app",
                table: "WorkflowStates");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                schema: "app",
                table: "Advancements",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
