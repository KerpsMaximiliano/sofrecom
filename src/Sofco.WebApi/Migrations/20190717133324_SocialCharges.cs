using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class SocialCharges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "app",
                table: "Budgets",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SocialCharges",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialCharges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialCharges_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialChargeItems",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountName = table.Column<string>(maxLength: 500, nullable: true),
                    AccountNumber = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    SocialChargeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialChargeItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialChargeItems_SocialCharges_SocialChargeId",
                        column: x => x.SocialChargeId,
                        principalSchema: "app",
                        principalTable: "SocialCharges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocialChargeItems_SocialChargeId",
                schema: "app",
                table: "SocialChargeItems",
                column: "SocialChargeId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialCharges_EmployeeId",
                schema: "app",
                table: "SocialCharges",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialChargeItems",
                schema: "app");

            migrationBuilder.DropTable(
                name: "SocialCharges",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "app",
                table: "Budgets");
        }
    }
}
