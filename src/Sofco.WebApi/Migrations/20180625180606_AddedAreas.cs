using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedAreas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                schema: "app",
                table: "PurchaseOrders",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_AreaId",
                schema: "app",
                table: "PurchaseOrders",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Areas_AreaId",
                schema: "app",
                table: "PurchaseOrders",
                column: "AreaId",
                principalSchema: "app",
                principalTable: "Areas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Areas_AreaId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Areas",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_AreaId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "AreaId",
                schema: "app",
                table: "PurchaseOrders");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                schema: "app",
                table: "PurchaseOrders",
                maxLength: 150,
                nullable: true);
        }
    }
}
