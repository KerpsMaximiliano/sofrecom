using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class Advancement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvancementReturnForms",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementReturnForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Advancements",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvancementReturnFormId = table.Column<int>(nullable: false),
                    AnalyticId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    PaymentForm = table.Column<int>(nullable: false),
                    StartDateReturn = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserApplicantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advancements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Advancements_AdvancementReturnForms_AdvancementReturnFormId",
                        column: x => x.AdvancementReturnFormId,
                        principalSchema: "app",
                        principalTable: "AdvancementReturnForms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Advancements_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvancementDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdvancementId = table.Column<int>(nullable: false),
                    Ammount = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvancementDetails_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_AdvancementReturnFormId",
                schema: "app",
                table: "Advancements",
                column: "AdvancementReturnFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_AnalyticId",
                schema: "app",
                table: "Advancements",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_CurrencyId",
                schema: "app",
                table: "Advancements",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Advancements_UserApplicantId",
                schema: "app",
                table: "Advancements",
                column: "UserApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementDetails_AdvancementId",
                schema: "app",
                table: "AdvancementDetails",
                column: "AdvancementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancementDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Advancements",
                schema: "app");

            migrationBuilder.DropTable(
                name: "AdvancementReturnForms",
                schema: "app");
        }
    }
}
