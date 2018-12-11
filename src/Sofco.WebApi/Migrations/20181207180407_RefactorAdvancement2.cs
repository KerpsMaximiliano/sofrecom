using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorAdvancement2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_AdvancementReturnForms_AdvancementReturnFormId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropTable(
                name: "AdvancementReturnForms",
                schema: "app");

            migrationBuilder.RenameColumn(
                name: "AdvancementReturnFormId",
                schema: "app",
                table: "Advancements",
                newName: "MonthsReturnId");

            migrationBuilder.RenameIndex(
                name: "IX_Advancements_AdvancementReturnFormId",
                schema: "app",
                table: "Advancements",
                newName: "IX_Advancements_MonthsReturnId");

            migrationBuilder.AddColumn<string>(
                name: "AdvancementReturnForm",
                schema: "app",
                table: "Advancements",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MonthsReturns",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthsReturns", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_MonthsReturns_MonthsReturnId",
                schema: "app",
                table: "Advancements",
                column: "MonthsReturnId",
                principalSchema: "app",
                principalTable: "MonthsReturns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Advancements_MonthsReturns_MonthsReturnId",
                schema: "app",
                table: "Advancements");

            migrationBuilder.DropTable(
                name: "MonthsReturns",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "AdvancementReturnForm",
                schema: "app",
                table: "Advancements");

            migrationBuilder.RenameColumn(
                name: "MonthsReturnId",
                schema: "app",
                table: "Advancements",
                newName: "AdvancementReturnFormId");

            migrationBuilder.RenameIndex(
                name: "IX_Advancements_MonthsReturnId",
                schema: "app",
                table: "Advancements",
                newName: "IX_Advancements_AdvancementReturnFormId");

            migrationBuilder.CreateTable(
                name: "AdvancementReturnForms",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementReturnForms", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Advancements_AdvancementReturnForms_AdvancementReturnFormId",
                schema: "app",
                table: "Advancements",
                column: "AdvancementReturnFormId",
                principalSchema: "app",
                principalTable: "AdvancementReturnForms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
