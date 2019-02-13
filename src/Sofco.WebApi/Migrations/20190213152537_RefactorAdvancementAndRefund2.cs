using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAdvancementAndRefund2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Refunds_RefundId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Refunds_RefundId",
                schema: "app",
                table: "Advancements",
                column: "RefundId",
                principalSchema: "app",
                principalTable: "Refunds",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Refunds_RefundId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_Refunds_RefundId",
                schema: "app",
                table: "Advancements",
                column: "RefundId",
                principalSchema: "app",
                principalTable: "Refunds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
