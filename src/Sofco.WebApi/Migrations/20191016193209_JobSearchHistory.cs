using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class JobSearchHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobSearchHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    StatusFromId = table.Column<int>(nullable: false),
                    StatusToId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 1000, nullable: true),
                    ReasonCauseId = table.Column<int>(nullable: false),
                    JobSearchId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobSearchHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobSearchHistories_JobSearchs_JobSearchId",
                        column: x => x.JobSearchId,
                        principalSchema: "app",
                        principalTable: "JobSearchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JobSearchHistories_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchHistories_JobSearchId",
                schema: "app",
                table: "JobSearchHistories",
                column: "JobSearchId");

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchHistories_ReasonCauseId",
                schema: "app",
                table: "JobSearchHistories",
                column: "ReasonCauseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobSearchHistories",
                schema: "app");
        }
    }
}
