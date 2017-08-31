﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AddedExternalHitoId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExternalId",
                table: "Hitos",
                newName: "ExternalProjectId");

            migrationBuilder.AddColumn<string>(
                name: "ExternalHitoId",
                table: "Hitos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalHitoId",
                table: "Hitos");

            migrationBuilder.RenameColumn(
                name: "ExternalProjectId",
                table: "Hitos",
                newName: "ExternalId");
        }
    }
}
