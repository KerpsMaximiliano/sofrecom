using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorSolfac : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImputationNumber2",
                table: "Solfacs");

            migrationBuilder.AlterColumn<string>(
                name: "ImputationNumber1",
                table: "Solfacs",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImputationNumber1",
                table: "Solfacs",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImputationNumber2",
                table: "Solfacs",
                maxLength: 10,
                nullable: true);
        }
    }
}
