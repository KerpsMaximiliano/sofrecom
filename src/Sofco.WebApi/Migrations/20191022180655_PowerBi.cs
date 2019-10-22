using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class PowerBi : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportsPowerBi",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Resource = table.Column<string>(maxLength: 200, nullable: true),
                    Seniority = table.Column<string>(maxLength: 200, nullable: true),
                    Profile = table.Column<string>(maxLength: 200, nullable: true),
                    Technology = table.Column<string>(maxLength: 200, nullable: true),
                    Manager = table.Column<string>(maxLength: 200, nullable: true),
                    Month1 = table.Column<decimal>(nullable: false),
                    Month2 = table.Column<decimal>(nullable: false),
                    Month3 = table.Column<decimal>(nullable: false),
                    Month4 = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportsPowerBi", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportsPowerBi",
                schema: "app");
        }
    }
}
