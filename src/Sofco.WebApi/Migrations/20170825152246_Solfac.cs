using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class Solfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Provinces",
                maxLength: 70,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ImputationNumbers",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "DocumentTypes",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Currencies",
                maxLength: 5,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Solfacs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    AttachedParts = table.Column<string>(maxLength: 500, nullable: true),
                    BuenosAiresPercentage = table.Column<decimal>(nullable: false),
                    CapitalPercentage = table.Column<decimal>(nullable: false),
                    CelPhone = table.Column<string>(maxLength: 15, nullable: true),
                    ClientName = table.Column<string>(maxLength: 100, nullable: true),
                    ContractNumber = table.Column<string>(maxLength: 50, nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    DocumentTypeId = table.Column<int>(nullable: false),
                    ImputationNumber1 = table.Column<string>(maxLength: 10, nullable: true),
                    ImputationNumber2 = table.Column<string>(maxLength: 10, nullable: true),
                    ImputationNumber3Id = table.Column<int>(nullable: false),
                    Iva21 = table.Column<decimal>(nullable: false),
                    ModifiedByUserId = table.Column<int>(nullable: false),
                    OtherProvince1Percentage = table.Column<decimal>(nullable: false),
                    OtherProvince2Percentage = table.Column<decimal>(nullable: false),
                    OtherProvince3Percentage = table.Column<decimal>(nullable: false),
                    ParticularSteps = table.Column<string>(maxLength: 500, nullable: true),
                    Project = table.Column<string>(maxLength: 100, nullable: true),
                    Province1Id = table.Column<int>(nullable: false),
                    Province2Id = table.Column<int>(nullable: false),
                    Province3Id = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TimeLimit = table.Column<short>(nullable: false),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    UserApplicantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solfacs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Solfacs_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hitos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    ExternalId = table.Column<string>(nullable: true),
                    Quantity = table.Column<short>(nullable: false),
                    SolfacId = table.Column<int>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    UnitPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hitos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hitos_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hitos_SolfacId",
                table: "Hitos",
                column: "SolfacId");

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_UserApplicantId",
                table: "Solfacs",
                column: "UserApplicantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hitos");

            migrationBuilder.DropTable(
                name: "Solfacs");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Provinces",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "ImputationNumbers",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "DocumentTypes",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Currencies",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 5,
                oldNullable: true);
        }
    }
}
