﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class UpdateLicense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                schema: "app",
                table: "Licenses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamDescription",
                schema: "app",
                table: "Licenses",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Final",
                schema: "app",
                table: "Licenses",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Parcial",
                schema: "app",
                table: "Licenses",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comments",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ExamDescription",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "Final",
                schema: "app",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "Parcial",
                schema: "app",
                table: "Licenses");
        }
    }
}
