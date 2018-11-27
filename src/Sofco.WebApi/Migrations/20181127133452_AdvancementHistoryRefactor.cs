using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class AdvancementHistoryRefactor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusFrom",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.DropColumn(
                name: "StatusTo",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.AddColumn<int>(
                name: "StatusFromId",
                schema: "app",
                table: "AdvancementHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusToId",
                schema: "app",
                table: "AdvancementHistories",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementHistories_StatusFromId",
                schema: "app",
                table: "AdvancementHistories",
                column: "StatusFromId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementHistories_StatusToId",
                schema: "app",
                table: "AdvancementHistories",
                column: "StatusToId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvancementHistories_WorkflowStates_StatusFromId",
                schema: "app",
                table: "AdvancementHistories",
                column: "StatusFromId",
                principalSchema: "app",
                principalTable: "WorkflowStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdvancementHistories_WorkflowStates_StatusToId",
                schema: "app",
                table: "AdvancementHistories",
                column: "StatusToId",
                principalSchema: "app",
                principalTable: "WorkflowStates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvancementHistories_WorkflowStates_StatusFromId",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_AdvancementHistories_WorkflowStates_StatusToId",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.DropIndex(
                name: "IX_AdvancementHistories_StatusFromId",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.DropIndex(
                name: "IX_AdvancementHistories_StatusToId",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.DropColumn(
                name: "StatusFromId",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.DropColumn(
                name: "StatusToId",
                schema: "app",
                table: "AdvancementHistories");

            migrationBuilder.AddColumn<string>(
                name: "StatusFrom",
                schema: "app",
                table: "AdvancementHistories",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatusTo",
                schema: "app",
                table: "AdvancementHistories",
                maxLength: 150,
                nullable: true);
        }
    }
}
