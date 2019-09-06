using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ResourceBilling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResourceBillings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProfileId = table.Column<int>(nullable: false),
                    SeniorityId = table.Column<int>(nullable: false),
                    PurchaseOrderId = table.Column<int>(nullable: false),
                    MonthHour = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    SubTotal = table.Column<decimal>(nullable: false),
                    ManagementReportBillingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceBillings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_ManagementReportBillings_ManagementReportBillingId",
                        column: x => x.ManagementReportBillingId,
                        principalSchema: "app",
                        principalTable: "ManagementReportBillings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalSchema: "app",
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalSchema: "app",
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResourceBillings_Seniorities_SeniorityId",
                        column: x => x.SeniorityId,
                        principalSchema: "app",
                        principalTable: "Seniorities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_ManagementReportBillingId",
                schema: "app",
                table: "ResourceBillings",
                column: "ManagementReportBillingId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_ProfileId",
                schema: "app",
                table: "ResourceBillings",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_PurchaseOrderId",
                schema: "app",
                table: "ResourceBillings",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceBillings_SeniorityId",
                schema: "app",
                table: "ResourceBillings",
                column: "SeniorityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResourceBillings",
                schema: "app");
        }
    }
}
