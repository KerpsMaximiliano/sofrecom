using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class CostDetail_Clases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostDetail",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IdAnalytic = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostDetail_Analytics_IdAnalytic",
                        column: x => x.IdAnalytic,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailResourceType",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostDetailResourceType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CostDetailHumanResource",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Month = table.Column<DateTime>(nullable: false),
                    Cost = table.Column<float>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CostDetailId = table.Column<int>(nullable: false)
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
                    Month = table.Column<DateTime>(nullable: false),
                    Cost = table.Column<float>(nullable: false),
                    TypeId = table.Column<int>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    CostDetailId = table.Column<int>(nullable: false)
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
                name: "IX_CostDetail_IdAnalytic",
                schema: "app",
                table: "CostDetail",
                column: "IdAnalytic");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostDetailHumanResource",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailResource",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetail",
                schema: "app");

            migrationBuilder.DropTable(
                name: "CostDetailResourceType",
                schema: "app");
        }
    }
}
