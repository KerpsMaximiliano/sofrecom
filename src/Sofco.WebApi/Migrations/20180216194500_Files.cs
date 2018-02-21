using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class Files : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Files",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedUser = table.Column<string>(maxLength: 50, nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    FileName = table.Column<string>(maxLength: 500, nullable: true),
                    FileType = table.Column<string>(maxLength: 10, nullable: true),
                    InternalFileName = table.Column<Guid>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderFiles",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AnalyticId = table.Column<int>(nullable: false),
                    Area = table.Column<string>(maxLength: 150, nullable: true),
                    ClientExternalId = table.Column<string>(maxLength: 150, nullable: true),
                    ClientExternalName = table.Column<string>(maxLength: 150, nullable: true),
                    CommercialManagerId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: true),
                    ManagerId = table.Column<int>(nullable: false),
                    Number = table.Column<string>(maxLength: 20, nullable: true),
                    ReceptionDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 150, nullable: true),
                    UpdateByUser = table.Column<string>(maxLength: 25, nullable: true),
                    UpdateDate = table.Column<DateTime>(nullable: false),
                    Year = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderFiles_Analytics_AnalyticId",
                        column: x => x.AnalyticId,
                        principalSchema: "app",
                        principalTable: "Analytics",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrderFiles_Users_CommercialManagerId",
                        column: x => x.CommercialManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PurchaseOrderFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderFiles_Users_ManagerId",
                        column: x => x.ManagerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderFiles_AnalyticId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "AnalyticId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderFiles_CommercialManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "CommercialManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderFiles_FileId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "FileId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderFiles_ManagerId",
                schema: "app",
                table: "PurchaseOrderFiles",
                column: "ManagerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseOrderFiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Files",
                schema: "app");
        }
    }
}
