using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CostTypeOnRefund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CostTypeId",
                schema: "app",
                table: "RefundDetails",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CostTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 200, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Category = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_CostTypeId",
                schema: "app",
                table: "RefundDetails",
                column: "CostTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_RefundDetails_CostTypes_CostTypeId",
                schema: "app",
                table: "RefundDetails",
                column: "CostTypeId",
                principalSchema: "app",
                principalTable: "CostTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefundDetails_CostTypes_CostTypeId",
                schema: "app",
                table: "RefundDetails");

            migrationBuilder.DropTable(
                name: "CostTypes",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_RefundDetails_CostTypeId",
                schema: "app",
                table: "RefundDetails");

            migrationBuilder.DropColumn(
                name: "CostTypeId",
                schema: "app",
                table: "RefundDetails");
        }
    }
}
