using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class contracted_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.CreateTable(
                name: "ContratedDetail",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdAnalytic = table.Column<int>(nullable: false),
                    AnalyticId = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    MonthYear = table.Column<DateTime>(nullable: false),
                    insurance = table.Column<float>(nullable: true),
                    honorary = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratedDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratedDetail_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratedDetail_AnalyticId",
                schema: "app",
                table: "ContratedDetail",
                column: "AnalyticId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContratedDetail",
                schema: "app");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                schema: "app",
                table: "CostDetail",
                maxLength: 200,
                nullable: true);
        }
    }
}
