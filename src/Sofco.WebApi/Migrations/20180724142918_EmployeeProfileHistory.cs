﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class EmployeeProfileHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeProfileHistory",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(nullable: true),
                    EmployeeData = table.Column<string>(nullable: true),
                    EmployeeNumber = table.Column<string>(maxLength: 50, nullable: true),
                    EmployeePreviousData = table.Column<string>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: true),
                    ModifiedFields = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProfileHistory", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeProfileHistory",
                schema: "app");
        }
    }
}
