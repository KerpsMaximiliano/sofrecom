﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class License : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Days",
                schema: "app",
                table: "LicenseTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExtraHolidaysQuantity",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasExtraHolidays",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HolidaysPending",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Licenses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Area = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    DaysQuantity = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    HasCertificate = table.Column<bool>(nullable: false),
                    ManagerId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    WithPayment = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Licenses_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Licenses_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Licenses_LicenseTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "app",
                        principalTable: "LicenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_EmployeeId",
                schema: "app",
                table: "Licenses",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_ManagerId",
                schema: "app",
                table: "Licenses",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_TypeId",
                schema: "app",
                table: "Licenses",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Licenses",
                schema: "app");

            migrationBuilder.DropColumn(
                name: "Days",
                schema: "app",
                table: "LicenseTypes");

            migrationBuilder.DropColumn(
                name: "ExtraHolidaysQuantity",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HasExtraHolidays",
                schema: "app",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "HolidaysPending",
                schema: "app",
                table: "Employees");
        }
    }
}
