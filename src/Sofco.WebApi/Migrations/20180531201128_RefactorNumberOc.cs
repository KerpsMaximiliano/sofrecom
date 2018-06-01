﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class RefactorNumberOc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                schema: "app",
                table: "PurchaseOrderFiles",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);
        }
    }
}
