﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class ProjectEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountId = table.Column<string>(maxLength: 200, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    CrmId = table.Column<string>(maxLength: 200, nullable: true),
                    Currency = table.Column<string>(maxLength: 200, nullable: true),
                    CurrencyId = table.Column<string>(maxLength: 200, nullable: true),
                    EndDate = table.Column<DateTime>(nullable: false),
                    Incomes = table.Column<decimal>(nullable: false),
                    Integrator = table.Column<string>(maxLength: 200, nullable: true),
                    IntegratorId = table.Column<string>(maxLength: 200, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    OpportunityId = table.Column<string>(maxLength: 200, nullable: true),
                    OpportunityName = table.Column<string>(maxLength: 200, nullable: true),
                    OpportunityNumber = table.Column<string>(maxLength: 200, nullable: true),
                    RealIncomes = table.Column<decimal>(nullable: false),
                    Remito = table.Column<bool>(nullable: false),
                    ServiceId = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    TotalAmmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Projects",
                schema: "app");
        }
    }
}
