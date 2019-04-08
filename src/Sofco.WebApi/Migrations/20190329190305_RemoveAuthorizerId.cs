using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RemoveAuthorizerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Users_AuthorizerId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Users_AuthorizerId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.RenameColumn(
                name: "AuthorizerId",
                schema: "app",
                table: "Refunds",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_AuthorizerId",
                schema: "app",
                table: "Refunds",
                newName: "IX_Refunds_UserId");

            migrationBuilder.RenameColumn(
                name: "AuthorizerId",
                schema: "app",
                table: "Advancements",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Advancements_AuthorizerId",
                schema: "app",
                table: "Advancements",
                newName: "IX_Advancements_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Users_UserId",
                schema: "app",
                table: "Advancements",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_UserId",
                schema: "app",
                table: "Refunds",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Users_UserId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_Users_UserId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "app",
                table: "Refunds",
                newName: "AuthorizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Refunds_UserId",
                schema: "app",
                table: "Refunds",
                newName: "IX_Refunds_AuthorizerId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "app",
                table: "Advancements",
                newName: "AuthorizerId");

            migrationBuilder.RenameIndex(
                name: "IX_Advancements_UserId",
                schema: "app",
                table: "Advancements",
                newName: "IX_Advancements_AuthorizerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Users_AuthorizerId",
                schema: "app",
                table: "Advancements",
                column: "AuthorizerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_Users_AuthorizerId",
                schema: "app",
                table: "Refunds",
                column: "AuthorizerId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
