using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class costsubtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailOthers_CostDetailTypes_CostDetailTypeId",
                schema: "app",
                table: "CostDetailOthers");

            migrationBuilder.RenameColumn(
                name: "CostDetailTypeId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "CostDetailSubtypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetailOthers_CostDetailTypeId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "IX_CostDetailOthers_CostDetailSubtypeId");
                  
            migrationBuilder.CreateTable(
                name: "CostDetailSubtype",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    TypeId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailSubtype", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailSubtype_CostDetailTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "app",
                        principalTable: "CostDetailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            
            migrationBuilder.CreateIndex(
                name: "IX_CostDetailSubtype_TypeId",
                schema: "app",
                table: "CostDetailSubtype",
                column: "TypeId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetailOthers_CostDetailSubtype_CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers");
            migrationBuilder.RenameColumn(
                name: "CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "CostDetailTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetailOthers_CostDetailSubtypeId",
                schema: "app",
                table: "CostDetailOthers",
                newName: "IX_CostDetailOthers_CostDetailTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetailOthers_CostDetailTypes_CostDetailTypeId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailTypeId",
                principalSchema: "app",
                principalTable: "CostDetailTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
