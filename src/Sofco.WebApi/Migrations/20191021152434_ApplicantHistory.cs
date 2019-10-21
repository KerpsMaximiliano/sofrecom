using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ApplicantHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicantHistories",
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
                    ApplicantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicantHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicantHistories_Applicants_ApplicantId",
                        column: x => x.ApplicantId,
                        principalSchema: "app",
                        principalTable: "Applicants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicantHistories_ReasonCauses_ReasonCauseId",
                        column: x => x.ReasonCauseId,
                        principalSchema: "app",
                        principalTable: "ReasonCauses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantHistories_ApplicantId",
                schema: "app",
                table: "ApplicantHistories",
                column: "ApplicantId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantHistories_ReasonCauseId",
                schema: "app",
                table: "ApplicantHistories",
                column: "ReasonCauseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicantHistories",
                schema: "app");
        }
    }
}
