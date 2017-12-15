﻿// <auto-generated/>
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdatedHitos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "app",
                table: "HitoDetails",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                schema: "app",
                table: "HitoDetails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                schema: "app",
                table: "Hitos",
                nullable: true,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyId",
                schema: "app",
                table: "Hitos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManagerId",
                schema: "app",
                table: "Hitos",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                schema: "app",
                table: "Hitos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OpportunityId",
                schema: "app",
                table: "Hitos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created",
                schema: "app",
                table: "HitoDetails");

            migrationBuilder.DropColumn(
                name: "Modified",
                schema: "app",
                table: "HitoDetails");

            migrationBuilder.DropColumn(
                name: "Created",
                schema: "app",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                schema: "app",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                schema: "app",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "Modified",
                schema: "app",
                table: "Hitos");

            migrationBuilder.DropColumn(
                name: "OpportunityId",
                schema: "app",
                table: "Hitos");
        }
    }
}
