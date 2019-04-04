using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class costdetailreorden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostDetailHumanResource",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailResource",
                schema: "app");

            migrationBuilder.AddColumn<float>(
                name: "Cost",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                schema: "app",
                table: "CostDetail",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModifiedById",
                schema: "app",
                table: "CostDetail",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MonthYear",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                schema: "app",
                table: "CostDetail",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                schema: "app",
                table: "CostDetail",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CostDetail_CreatedById",
                schema: "app",
                table: "CostDetail",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetail_EmployeeId",
                schema: "app",
                table: "CostDetail",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetail_ModifiedById",
                schema: "app",
                table: "CostDetail",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetail_TypeId",
                schema: "app",
                table: "CostDetail",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetail_UserId",
                schema: "app",
                table: "CostDetail",
                column: "UserId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Users_CreatedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_CostDetail_Employees_EmployeeId",
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

            migrationBuilder.DropIndex(
                name: "IX_CostDetail_CreatedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropIndex(
                name: "IX_CostDetail_EmployeeId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropIndex(
                name: "IX_CostDetail_ModifiedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropIndex(
                name: "IX_CostDetail_TypeId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropIndex(
                name: "IX_CostDetail_UserId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "Cost",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "ModifiedById",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "MonthYear",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "TypeId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "app",
                table: "CostDetail");

            migrationBuilder.CreateTable(
                name: "CostDetailHumanResource",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cost = table.Column<float>(nullable: false),
                    CostDetailId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    Month = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailHumanResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailHumanResource_CostDetail_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailHumanResource_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostDetailHumanResource_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalSchema: "app",
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostDetailHumanResource_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostDetailHumanResource_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailResource",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Cost = table.Column<float>(nullable: false),
                    CostDetailId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: true),
                    ModifiedById = table.Column<int>(nullable: true),
                    Month = table.Column<DateTime>(nullable: false),
                    TypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailResource", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetailResource_CostDetail_CostDetailId",
                        column: x => x.CostDetailId,
                        principalSchema: "app",
                        principalTable: "CostDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CostDetailResource_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostDetailResource_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostDetailResource_CostDetailResourceType_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "app",
                        principalTable: "CostDetailResourceType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailHumanResource_CostDetailId",
                schema: "app",
                table: "CostDetailHumanResource",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailHumanResource_CreatedById",
                schema: "app",
                table: "CostDetailHumanResource",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailHumanResource_EmployeeId",
                schema: "app",
                table: "CostDetailHumanResource",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailHumanResource_ModifiedById",
                schema: "app",
                table: "CostDetailHumanResource",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailHumanResource_UserId",
                schema: "app",
                table: "CostDetailHumanResource",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResource_CostDetailId",
                schema: "app",
                table: "CostDetailResource",
                column: "CostDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResource_CreatedById",
                schema: "app",
                table: "CostDetailResource",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResource_ModifiedById",
                schema: "app",
                table: "CostDetailResource",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_CostDetailResource_TypeId",
                schema: "app",
                table: "CostDetailResource",
                column: "TypeId");
        }
    }
}
