using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class staffreport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BudgetType",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailCategories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailSubcategories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    CostDetailCategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailSubcategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailSubcategories_CostDetailCategories_CostDetailCategoryId",
                        column: x => x.CostDetailCategoryId,
                        principalSchema: "app",
                        principalTable: "CostDetailCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailStaff",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Value = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    CostDetailId = table.Column<int>(nullable: false),
                    CostDetailSubcategoryId = table.Column<int>(nullable: false),
                    BudgetTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailStaff", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_BudgetType_BudgetTypeId",
                        column: x => x.BudgetTypeId,
                        principalSchema: "app",
                        principalTable: "BudgetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailStaff_CostDetailSubcategories_CostDetailSubcategoryId",
                        column: x => x.CostDetailSubcategoryId,
                        principalSchema: "app",
                        principalTable: "CostDetailSubcategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_BudgetTypeId",
                schema: "app",
                table: "CostDetailStaff",
                column: "BudgetTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_CostDetailId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailStaff_CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailStaff",
                column: "CostDetailSubcategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailSubcategories_CostDetailCategoryId",
                schema: "app",
                table: "CostDetailSubcategories",
                column: "CostDetailCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostDetailStaff",
                schema: "app");

            migrationBuilder.DropTable(
                name: "BudgetType",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailSubcategories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailCategories",
                schema: "app");
        }
    }
}
