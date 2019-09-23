using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class deletetypesubtypecostDetailOther : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailOthers_CostDetailSubtype_CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers");

            migrationBuilder.DropTable(
                name: "CostDetailSubtype",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailTypes",
                schema: "app");

            migrationBuilder.RenameColumn(
                name: "CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "CostDetailSubcategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetailOthers_CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "IX_CostDetailOthers_CostDetailSubcategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailOthers_CostDetailSubcategories_CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailSubcategoryId",
                principalSchema: "app",
                principalTable: "CostDetailSubcategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailOthers_CostDetailSubcategories_CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailOthers");

            migrationBuilder.RenameColumn(
                name: "CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "CostDetailSubtypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetailOthers_CostDetailSubcategoryId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "IX_CostDetailOthers_CostDetailSubtypeId");

            migrationBuilder.CreateTable(
                name: "CostDetailTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BelongEmployee = table.Column<bool>(nullable: false),
                    Default = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailSubtype",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailTypeId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailSubtype", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailSubtype_CostDetailTypes_CostDetailTypeId",
                        column: x => x.CostDetailTypeId,
                        principalSchema: "app",
                        principalTable: "CostDetailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailSubtype_CostDetailTypeId",
                schema: "app",
                table: "CostDetailSubtype",
                column: "CostDetailTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailOthers_CostDetailSubtype_CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailSubtypeId",
                principalSchema: "app",
                principalTable: "CostDetailSubtype",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
