using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeManagementReportBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ValueEvalProp",
                schema: "app",
                table: "ManagementReportBillings",
                newName: "EvalPropExpenseValue");

            migrationBuilder.AddColumn<decimal>(
                name: "EvalPropBillingValue",
                schema: "app",
                table: "ManagementReportBillings",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvalPropBillingValue",
                schema: "app",
                table: "ManagementReportBillings");

            migrationBuilder.RenameColumn(
                name: "EvalPropExpenseValue",
                schema: "app",
                table: "ManagementReportBillings",
                newName: "ValueEvalProp");
        }
    }
}
