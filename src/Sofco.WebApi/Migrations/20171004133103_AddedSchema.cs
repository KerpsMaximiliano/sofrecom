﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "app");

            migrationBuilder.RenameTable(
                name: "Provinces",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ImputationNumbers",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "UserGroup",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "RoleFunctionality",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Customers",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "SolfacHistories",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "SolfacAttachments",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Solfacs",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "InvoiceHistories",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Invoices",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Hitos",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Users",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Roles",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Modules",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Menus",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Groups",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "Functionalities",
                newSchema: "app");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Provinces",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "ImputationNumbers",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "DocumentTypes",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Currencies",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "UserGroup",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "RoleFunctionality",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Customers",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "SolfacHistories",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "SolfacAttachments",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Solfacs",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "InvoiceHistories",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Invoices",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Hitos",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Users",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Roles",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Modules",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Menus",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Groups",
                schema: "app");

            migrationBuilder.RenameTable(
                name: "Functionalities",
                schema: "app");
        }
    }
}
