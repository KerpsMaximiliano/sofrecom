using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ManagementReportComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ManagementReportComments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Comment = table.Column<string>(maxLength: 2000, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    ManagementReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementReportComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementReportComments_ManagementReports_ManagementReportId",
                        column: x => x.ManagementReportId,
                        principalSchema: "app",
                        principalTable: "ManagementReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReportComments_ManagementReportId",
                schema: "app",
                table: "ManagementReportComments",
                column: "ManagementReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ManagementReportComments",
                schema: "app");
        }
    }
}
