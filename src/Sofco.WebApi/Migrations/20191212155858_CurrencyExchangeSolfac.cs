using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CurrencyExchangeSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyExchange",
                schema: "app",
                table: "Solfacs",
                type: "decimal(18, 4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CurrencyExchange",
                schema: "app",
                table: "Solfacs",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18, 4)",
                oldNullable: true);
        }
    }
}
