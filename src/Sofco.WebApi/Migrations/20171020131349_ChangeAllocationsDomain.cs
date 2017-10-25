using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class ChangeAllocationsDomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_AnalyticId",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "InitialDate",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "ResourceName",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropColumn(
                name: "ResourceSenority",
                schema: "app",
                table: "Allocations");

            migrationBuilder.RenameColumn(
                name: "RealPercentage",
                schema: "app",
                table: "Allocations",
                newName: "EmployeeId");

            migrationBuilder.AlterColumn<bool>(
                name: "SoftwareLaw",
                schema: "app",
                table: "Analytics",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<decimal>(
                name: "Percentage",
                schema: "app",
                table: "Allocations",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations",
                columns: new[] { "AnalyticId", "EmployeeId" });

            migrationBuilder.CreateTable(
                name: "Employees",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BillingPercentage = table.Column<decimal>(nullable: false),
                    BirthDay = table.Column<DateTime>(nullable: false),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Profile = table.Column<string>(maxLength: 100, nullable: true),
                    Seniority = table.Column<string>(maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    Technology = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_EmployeeId",
                schema: "app",
                table: "Allocations",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Allocations_Employees_EmployeeId",
                schema: "app",
                table: "Allocations",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Allocations_Employees_EmployeeId",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropTable(
                name: "Employees",
                schema: "app");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations");

            migrationBuilder.DropIndex(
                name: "IX_Allocations_EmployeeId",
                schema: "app",
                table: "Allocations");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                schema: "app",
                table: "Allocations",
                newName: "RealPercentage");

            migrationBuilder.AlterColumn<bool>(
                name: "SoftwareLaw",
                schema: "app",
                table: "Analytics",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                schema: "app",
                table: "Allocations",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "app",
                table: "Allocations",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "InitialDate",
                schema: "app",
                table: "Allocations",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ResourceId",
                schema: "app",
                table: "Allocations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceName",
                schema: "app",
                table: "Allocations",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResourceSenority",
                schema: "app",
                table: "Allocations",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Allocations",
                schema: "app",
                table: "Allocations",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_AnalyticId",
                schema: "app",
                table: "Allocations",
                column: "AnalyticId");
        }
    }
}
