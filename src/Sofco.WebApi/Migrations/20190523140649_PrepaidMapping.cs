using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class PrepaidMapping : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prepaid",
                schema: "app",
                table: "Employees",
                newName: "PrepaidPlan");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "app",
                table: "Prepaids",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                schema: "app",
                table: "Prepaids",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PrepaidImportedData",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Prepaid = table.Column<string>(maxLength: 100, nullable: true),
                    Period = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EmployeeName = table.Column<string>(maxLength: 100, nullable: true),
                    EmployeeNumber = table.Column<string>(maxLength: 15, nullable: true),
                    PrepaidBeneficiaries = table.Column<int>(nullable: false),
                    TigerBeneficiaries = table.Column<int>(nullable: false),
                    PrepaidPlan = table.Column<string>(maxLength: 100, nullable: true),
                    TigerPlan = table.Column<string>(maxLength: 100, nullable: true),
                    PrepaidCost = table.Column<decimal>(nullable: false),
                    TigerCost = table.Column<decimal>(nullable: false),
                    Dni = table.Column<string>(maxLength: 15, nullable: true),
                    Cuil = table.Column<string>(maxLength: 15, nullable: true),
                    Comments = table.Column<string>(maxLength: 500, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrepaidImportedData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrepaidImportedData",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "Code",
                schema: "app",
                table: "Prepaids");

            migrationBuilder.RenameColumn(
                name: "PrepaidPlan",
                schema: "app",
                table: "Employees",
                newName: "Prepaid");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                schema: "app",
                table: "Prepaids",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 250,
                oldNullable: true);
        }
    }
}
