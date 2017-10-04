using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorRolesYFuncionaliades : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "Functionalities",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "RoleFunctionality",
                columns: table => new
                {
                    RoleId = table.Column<int>(nullable: false),
                    FunctionalityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleFunctionality", x => new { x.RoleId, x.FunctionalityId });
                    table.ForeignKey(
                        name: "FK_RoleFunctionality_Functionalities_FunctionalityId",
                        column: x => x.FunctionalityId,
                        principalTable: "Functionalities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleFunctionality_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_CurrencyId",
                table: "Solfacs",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_ImputationNumber3Id",
                table: "Solfacs",
                column: "ImputationNumber3Id");

            migrationBuilder.CreateIndex(
                name: "IX_Functionalities_ModuleId",
                table: "Functionalities",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleFunctionality_FunctionalityId",
                table: "RoleFunctionality",
                column: "FunctionalityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Functionalities_Modules_ModuleId",
                table: "Functionalities",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_Currencies_CurrencyId",
                table: "Solfacs",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_ImputationNumbers_ImputationNumber3Id",
                table: "Solfacs",
                column: "ImputationNumber3Id",
                principalTable: "ImputationNumbers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Functionalities_Modules_ModuleId",
                table: "Functionalities");

            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_Currencies_CurrencyId",
                table: "Solfacs");

            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_ImputationNumbers_ImputationNumber3Id",
                table: "Solfacs");

            migrationBuilder.DropTable(
                name: "RoleFunctionality");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_CurrencyId",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_ImputationNumber3Id",
                table: "Solfacs");

            migrationBuilder.DropIndex(
                name: "IX_Functionalities_ModuleId",
                table: "Functionalities");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Functionalities");
        }
    }
}
