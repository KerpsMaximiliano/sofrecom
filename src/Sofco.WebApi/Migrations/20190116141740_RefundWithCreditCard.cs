using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefundWithCreditCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreditCardId",
                schema: "app",
                table: "Refunds",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CreditCards",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditCards", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CreditCardId",
                schema: "app",
                table: "Refunds",
                column: "CreditCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Refunds_CreditCards_CreditCardId",
                schema: "app",
                table: "Refunds",
                column: "CreditCardId",
                principalSchema: "app",
                principalTable: "CreditCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Refunds_CreditCards_CreditCardId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropTable(
                name: "CreditCards",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Refunds_CreditCardId",
                schema: "app",
                table: "Refunds");

            migrationBuilder.DropColumn(
                name: "CreditCardId",
                schema: "app",
                table: "Refunds");
        }
    }
}
