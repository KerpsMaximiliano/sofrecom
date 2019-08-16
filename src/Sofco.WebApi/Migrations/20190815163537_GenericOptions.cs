using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class GenericOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Technologies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Solutions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "SoftwareLaws",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "ServiceTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "PurchaseOrderOptions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Provinces",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Products",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Prepaids",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "PaymentTerms",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "MonthsReturns",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "ImputationNumbers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "EmployeeProfile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "EmployeeEndReason",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "DocumentTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "Currencies",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                schema: "app",
                table: "ClientGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Profiles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seniorities",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seniorities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Seniorities",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Skills",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Technologies");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Solutions");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "SoftwareLaws");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "PurchaseOrderOptions");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Provinces");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Prepaids");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "PaymentTerms");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "MonthsReturns");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "ImputationNumbers");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "EmployeeProfile");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "EmployeeEndReason");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "Active",
                schema: "app",
                table: "ClientGroups");
        }
    }
}
