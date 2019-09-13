using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorResourceBilling2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Employees_EmployeeId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Profiles_ProfileId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                schema: "app",
                table: "ResourceBillings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "Profile",
                schema: "app",
                table: "ResourceBillings",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Employees_EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Profiles_ProfileId",
                schema: "app",
                table: "ResourceBillings",
                column: "ProfileId",
                principalSchema: "app",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Employees_EmployeeId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.DropForeignKey(
                name: "FK_ResourceBillings_Profiles_ProfileId",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.DropColumn(
                name: "Profile",
                schema: "app",
                table: "ResourceBillings");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                schema: "app",
                table: "ResourceBillings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Employees_EmployeeId",
                schema: "app",
                table: "ResourceBillings",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResourceBillings_Profiles_ProfileId",
                schema: "app",
                table: "ResourceBillings",
                column: "ProfileId",
                principalSchema: "app",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
