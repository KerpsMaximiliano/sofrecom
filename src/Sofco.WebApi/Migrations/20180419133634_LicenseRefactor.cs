﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class LicenseRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DaysQuantityByLaw",
                schema: "app",
                table: "Licenses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HolidaysPendingByLaw",
                schema: "app",
                table: "Employees",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DaysQuantityByLaw",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "HolidaysPendingByLaw",
                schema: "app",
                table: "Employees");
        }
    }
}
