using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class PaymentTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentTermId",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Solfacs_PaymentTermId",
                schema: "app",
                table: "Solfacs",
                column: "PaymentTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs",
                column: "PaymentTermId",
                principalSchema: "app",
                principalTable: "PaymentTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropTable(
                name: "PaymentTerms",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_Solfacs_PaymentTermId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.DropColumn(
                name: "PaymentTermId",
                schema: "app",
                table: "Solfacs");
        }
    }
}
