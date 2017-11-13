using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class PaymentTermRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTermId",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: 1,
                oldDefaultValue: 1,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs",
                column: "PaymentTermId",
                principalSchema: "app",
                principalTable: "PaymentTerms",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solfacs_PaymentTerms_PaymentTermId",
                schema: "app",
                table: "Solfacs");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentTermId",
                schema: "app",
                table: "Solfacs",
                nullable: false,
                defaultValue: 1,
                oldDefaultValue: 1,
                oldClrType: typeof(int));

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
    }
}
