using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class OpportunityTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Opportunities",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Number = table.Column<string>(maxLength: 50, nullable: true),
                    ActualValue = table.Column<decimal>(nullable: true),
                    ContactId = table.Column<string>(maxLength: 200, nullable: true),
                    ParentContactName = table.Column<string>(maxLength: 200, nullable: true),
                    ProjectManagerId = table.Column<string>(maxLength: 200, nullable: true),
                    ProjectManagerName = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Opportunities", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Opportunities",
                schema: "app");
        }
    }
}
