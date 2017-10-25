using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class TimeManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Solutions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Solutions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technologies",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Analytics",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityId = table.Column<int>(nullable: true),
                    AmountEarned = table.Column<string>(maxLength: 500, nullable: true),
                    AmountProject = table.Column<string>(maxLength: 500, nullable: true),
                    BugsAccess = table.Column<bool>(nullable: false),
                    ClientExternalId = table.Column<string>(maxLength: 150, nullable: true),
                    ClientExternalName = table.Column<string>(maxLength: 150, nullable: true),
                    ClientGroup = table.Column<int>(nullable: true),
                    ClientProjectTfs = table.Column<string>(maxLength: 150, nullable: true),
                    CommercialManager = table.Column<string>(maxLength: 150, nullable: true),
                    ContractNumber = table.Column<string>(maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    DirectorId = table.Column<int>(nullable: false),
                    EndDateContract = table.Column<DateTime>(nullable: false),
                    EvalProp = table.Column<bool>(nullable: true),
                    ManagerId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Product = table.Column<int>(nullable: true),
                    Proposal = table.Column<string>(maxLength: 200, nullable: true),
                    PurchaseOrder = table.Column<int>(nullable: false),
                    Service = table.Column<string>(maxLength: 50, nullable: true),
                    SoftwareLaw = table.Column<bool>(nullable: false),
                    SolutionId = table.Column<int>(nullable: true),
                    StartDateContract = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    TechnologyId = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 150, nullable: true),
                    UsersQv = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Analytics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Analytics_ImputationNumbers_ActivityId",
                        column: x => x.ActivityId,
                        principalSchema: "app",
                        principalTable: "ImputationNumbers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Solutions_SolutionId",
                        column: x => x.SolutionId,
                        principalSchema: "app",
                        principalTable: "Solutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Analytics_Technologies_TechnologyId",
                        column: x => x.TechnologyId,
                        principalSchema: "app",
                        principalTable: "Technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalyticId = table.Column<int>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    InitialDate = table.Column<DateTime>(nullable: false),
                    Percentage = table.Column<int>(nullable: false),
                    RealPercentage = table.Column<int>(nullable: false),
                    ResourceId = table.Column<string>(maxLength: 150, nullable: true),
                    ResourceName = table.Column<string>(maxLength: 150, nullable: true),
                    ResourceSenority = table.Column<string>(maxLength: 20, nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Allocations_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_AnalyticId",
                schema: "app",
                table: "Allocations",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_ActivityId",
                schema: "app",
                table: "Analytics",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_CurrencyId",
                schema: "app",
                table: "Analytics",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_SolutionId",
                schema: "app",
                table: "Analytics",
                column: "SolutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Analytics_TechnologyId",
                schema: "app",
                table: "Analytics",
                column: "TechnologyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Analytics",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Solutions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Technologies",
                schema: "app");
        }
    }
}
