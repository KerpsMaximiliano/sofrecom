﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class SolfacIntegrator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Integrator",
                schema: "app",
                table: "Solfacs",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IntegratorId",
                schema: "app",
                table: "Solfacs",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Integrator",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "IntegratorId",
                schema: "app",
                table: "Solfacs");
        }
    }
}
