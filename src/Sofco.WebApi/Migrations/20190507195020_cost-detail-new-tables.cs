using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class costdetailnewtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractedDetail_Analytics_IdAnalytic",
                schema: "app",
                table: "ContractedDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Users_CreatedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Employees_EmployeeId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Analytics_IdAnalytic",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Users_ModifiedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_CostDetailResourceType_TypeId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Users_UserId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropTable(
                name: "CostDetailResourceType",
                schema: "app");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostDetail",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropIndex(
                name: "IX_CostDetail_IdAnalytic",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractedDetail",
                schema: "app",
                table: "ContractedDetail");

            migrationBuilder.DropColumn(
                name: "Adjustment",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "Charges",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "Cost",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "IdAnalytic",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.RenameTable(
                name: "CostDetail",
                schema: "app",
                newName: "CostDetails",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ContractedDetail",
                schema: "app",
                newName: "ContractedDetails",
                newSchema: "app");

            migrationBuilder.RenameColumn(
                name: "TypeId",
                schema: "app",
                table: "CostDetails",
                newName: "ManagementReportId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetail_UserId",
                schema: "app",
                table: "CostDetails",
                newName: "IX_CostDetails_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetail_TypeId",
                schema: "app",
                table: "CostDetails",
                newName: "IX_CostDetails_ManagementReportId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetail_ModifiedById",
                schema: "app",
                table: "CostDetails",
                newName: "IX_CostDetails_ModifiedById");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetail_EmployeeId",
                schema: "app",
                table: "CostDetails",
                newName: "IX_CostDetails_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetail_CreatedById",
                schema: "app",
                table: "CostDetails",
                newName: "IX_CostDetails_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ContractedDetail_IdAnalytic",
                schema: "app",
                table: "ContractedDetails",
                newName: "IX_ContractedDetails_IdAnalytic");

            migrationBuilder.AddColumn<int>(
                name: "AnalyticId",
                schema: "app",
                table: "CostDetails",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostDetails",
                schema: "app",
                table: "CostDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractedDetails",
                schema: "app",
                table: "ContractedDetails",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CostDetailProfiles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    EmployeeProfileId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailProfiles_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailProfiles_EmployeeProfile_EmployeeProfileId",
                        column: x => x.EmployeeProfileId,
                        principalSchema: "app",
                        principalTable: "EmployeeProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailResources",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Adjustment = table.Column<decimal>(nullable: false),
                    Charges = table.Column<decimal>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_Users_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_Employees_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailResources_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    Default = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ManagementReportBillings",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ManagementReportId = table.Column<int>(nullable: false),
                    MonthYear = table.Column<DateTime>(nullable: false),
                    ValueEvalProp = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ManagementReportBillings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ManagementReportBillings_ManagementReports_ManagementReportId",
                        column: x => x.ManagementReportId,
                        principalSchema: "app",
                        principalTable: "ManagementReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailOthers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CostDetailId = table.Column<int>(nullable: false),
                    CostDetailTypeId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailOthers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailOthers_CostDetails_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailOthers_CostDetailTypes_CostDetailTypeId",
                        column: x => x.CostDetailTypeId,
                        principalSchema: "app",
                        principalTable: "CostDetailTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostDetails_AnalyticId",
                schema: "app",
                table: "CostDetails",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailOthers_CostDetailId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailOthers_CostDetailTypeId",
                schema: "app",
                table: "CostDetailOthers",
                column: "CostDetailTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailProfiles_CostDetailId",
                schema: "app",
                table: "CostDetailProfiles",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailProfiles_EmployeeProfileId",
                schema: "app",
                table: "CostDetailProfiles",
                column: "EmployeeProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResources_CostDetailId",
                schema: "app",
                table: "CostDetailResources",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ManagementReportBillings_ManagementReportId",
                schema: "app",
                table: "ManagementReportBillings",
                column: "ManagementReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractedDetails_Analytics_IdAnalytic",
                schema: "app",
                table: "ContractedDetails",
                column: "IdAnalytic",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetails_Analytics_AnalyticId",
                schema: "app",
                table: "CostDetails",
                column: "AnalyticId",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetails_Users_CreatedById",
                schema: "app",
                table: "CostDetails",
                column: "CreatedById",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetails_Employees_EmployeeId",
                schema: "app",
                table: "CostDetails",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetails_ManagementReports_ManagementReportId",
                schema: "app",
                table: "CostDetails",
                column: "ManagementReportId",
                principalSchema: "app",
                principalTable: "ManagementReports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetails_Users_ModifiedById",
                schema: "app",
                table: "CostDetails",
                column: "ModifiedById",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetails_Users_UserId",
                schema: "app",
                table: "CostDetails",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractedDetails_Analytics_IdAnalytic",
                schema: "app",
                table: "ContractedDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetails_Analytics_AnalyticId",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetails_Users_CreatedById",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetails_Employees_EmployeeId",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetails_ManagementReports_ManagementReportId",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetails_Users_ModifiedById",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetails_Users_UserId",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropTable(
                name: "CostDetailOthers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailProfiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailResources",
                schema: "app");

            migrationBuilder.DropTable(
                name: "ManagementReportBillings",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailTypes",
                schema: "app");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostDetails",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropIndex(
                name: "IX_CostDetails_AnalyticId",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContractedDetails",
                schema: "app",
                table: "ContractedDetails");

            migrationBuilder.DropColumn(
                name: "AnalyticId",
                schema: "app",
                table: "CostDetails");

            migrationBuilder.RenameTable(
                name: "CostDetails",
                schema: "app",
                newName: "CostDetail",
                newSchema: "app");

            migrationBuilder.RenameTable(
                name: "ContractedDetails",
                schema: "app",
                newName: "ContractedDetail",
                newSchema: "app");

            migrationBuilder.RenameColumn(
                name: "ManagementReportId",
                schema: "app",
                table: "CostDetail",
                newName: "TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetails_UserId",
                schema: "app",
                table: "CostDetail",
                newName: "IX_CostDetail_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetails_ModifiedById",
                schema: "app",
                table: "CostDetail",
                newName: "IX_CostDetail_ModifiedById");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetails_ManagementReportId",
                schema: "app",
                table: "CostDetail",
                newName: "IX_CostDetail_TypeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetails_EmployeeId",
                schema: "app",
                table: "CostDetail",
                newName: "IX_CostDetail_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_CostDetails_CreatedById",
                schema: "app",
                table: "CostDetail",
                newName: "IX_CostDetail_CreatedById");

            migrationBuilder.RenameIndex(
                name: "IX_ContractedDetails_IdAnalytic",
                schema: "app",
                table: "ContractedDetail",
                newName: "IX_ContractedDetail_IdAnalytic");

            migrationBuilder.AddColumn<float>(
                name: "Adjustment",
                schema: "app",
                table: "CostDetail",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Charges",
                schema: "app",
                table: "CostDetail",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Cost",
                schema: "app",
                table: "CostDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdAnalytic",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostDetail",
                schema: "app",
                table: "CostDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContractedDetail",
                schema: "app",
                table: "ContractedDetail",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CostDetailResourceType",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Default = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailResourceType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostDetail_IdAnalytic",
                schema: "app",
                table: "CostDetail",
                column: "IdAnalytic");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractedDetail_Analytics_IdAnalytic",
                schema: "app",
                table: "ContractedDetail",
                column: "IdAnalytic",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetail_Users_CreatedById",
                schema: "app",
                table: "CostDetail",
                column: "CreatedById",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetail_Employees_EmployeeId",
                schema: "app",
                table: "CostDetail",
                column: "EmployeeId",
                principalSchema: "app",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetail_Analytics_IdAnalytic",
                schema: "app",
                table: "CostDetail",
                column: "IdAnalytic",
                principalSchema: "app",
                principalTable: "Analytics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetail_Users_ModifiedById",
                schema: "app",
                table: "CostDetail",
                column: "ModifiedById",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetail_CostDetailResourceType_TypeId",
                schema: "app",
                table: "CostDetail",
                column: "TypeId",
                principalSchema: "app",
                principalTable: "CostDetailResourceType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CostDetail_Users_UserId",
                schema: "app",
                table: "CostDetail",
                column: "UserId",
                principalSchema: "app",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
