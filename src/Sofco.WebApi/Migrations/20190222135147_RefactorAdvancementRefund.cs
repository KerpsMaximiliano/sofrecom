using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAdvancementRefund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_Refunds_RefundId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropIndex(
                name: "IX_Advancements_RefundId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropColumn(
                name: "RefundId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.AddColumn<bool>(
                name: "CashReturn",
                schema: "app",
                table: "Refunds",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AdvancementRefunds",
                schema: "app",
                columns: table => new
                {
                    AdvancementId = table.Column<int>(nullable: false),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementRefunds", x => new { x.AdvancementId, x.RefundId });
                    table.ForeignKey(
                        name: "FK_AdvancementRefunds_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvancementRefunds_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementRefunds_RefundId",
                schema: "app",
                table: "AdvancementRefunds",
                column: "RefundId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancementRefunds",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "CashReturn",
                schema: "app",
                table: "Refunds");

            migrationBuilder.AddColumn<int>(
                name: "RefundId",
                schema: "app",
                table: "Advancements",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_RefundId",
                schema: "app",
                table: "Advancements",
                column: "RefundId");

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
    }
}
