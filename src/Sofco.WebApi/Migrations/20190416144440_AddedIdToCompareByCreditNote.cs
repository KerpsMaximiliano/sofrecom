using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedIdToCompareByCreditNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdToCompareByCreditNote",
                schema: "app",
                table: "Solfacs",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalAmount",
                schema: "app",
                table: "Hitos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdToCompareByCreditNote",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "OriginalAmount",
                schema: "app",
                table: "Hitos");
        }
    }
}
