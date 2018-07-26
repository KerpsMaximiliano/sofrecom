﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class ServiceEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<string>(maxLength: 200, nullable: true),
                    AccountName = table.Column<string>(maxLength: 200, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Analytic = table.Column<string>(maxLength: 200, nullable: true),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Industry = table.Column<string>(maxLength: 200, nullable: true),
                    Manager = table.Column<string>(maxLength: 200, nullable: true),
                    ManagerId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceType = table.Column<string>(maxLength: 200, nullable: true),
                    ServiceTypeId = table.Column<int>(nullable: false),
                    SolutionType = table.Column<string>(maxLength: 200, nullable: true),
                    SolutionTypeId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    TechnologyType = table.Column<string>(maxLength: 200, nullable: true),
                    TechnologyTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Services",
                schema: "app");
        }
    }
}
