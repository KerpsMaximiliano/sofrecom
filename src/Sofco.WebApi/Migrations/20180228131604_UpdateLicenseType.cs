﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateLicenseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                schema: "app",
                table: "LicenseTypes");

            migrationBuilder.DropColumn(
                name: "LicenseTypeNumber",
                schema: "app",
                table: "LicenseTypes");

            migrationBuilder.DropColumn(
                name: "Modified",
                schema: "app",
                table: "LicenseTypes");

            migrationBuilder.AddColumn<bool>(
                name: "WithPayment",
                schema: "app",
                table: "LicenseTypes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WithPayment",
                schema: "app",
                table: "LicenseTypes");

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "app",
                table: "LicenseTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LicenseTypeNumber",
                schema: "app",
                table: "LicenseTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                schema: "app",
                table: "LicenseTypes",
                nullable: true);
        }
    }
}
