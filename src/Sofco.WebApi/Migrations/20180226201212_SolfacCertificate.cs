using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class SolfacCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolfacCertificates",
                schema: "app",
                columns: table => new
                {
                    SolfacId = table.Column<int>(nullable: false),
                    CertificateId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolfacCertificates", x => new { x.SolfacId, x.CertificateId });
                    table.ForeignKey(
                        name: "FK_SolfacCertificates_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalSchema: "app",
                        principalTable: "Certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SolfacCertificates_Solfacs_SolfacId",
                        column: x => x.SolfacId,
                        principalSchema: "app",
                        principalTable: "Solfacs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SolfacCertificates_CertificateId",
                schema: "app",
                table: "SolfacCertificates",
                column: "CertificateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolfacCertificates",
                schema: "app");
        }
    }
}
