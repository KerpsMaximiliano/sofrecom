using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class Refund : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Refunds",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserApplicantId = table.Column<int>(nullable: false),
                    AuthorizerId = table.Column<int>(nullable: true),
                    StatusId = table.Column<int>(nullable: false),
                    InWorkflowProcess = table.Column<bool>(nullable: false),
                    Contract = table.Column<string>(maxLength: 300, nullable: true),
                    CurrencyId = table.Column<int>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    TotalAmmount = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Refunds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Refunds_Users_AuthorizerId",
                        column: x => x.AuthorizerId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalSchema: "app",
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_WorkflowStates_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Refunds_Users_UserApplicantId",
                        column: x => x.UserApplicantId,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvancementRefunds",
                schema: "app",
                columns: table => new
                {
                    AdvancementId = table.Column<int>(nullable: false),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvancementRefunds", x => new { x.AdvancementId, x.RefundId });
                    table.ForeignKey(
                        name: "FK_AdvancementRefunds_Advancements_AdvancementId",
                        column: x => x.AdvancementId,
                        principalSchema: "app",
                        principalTable: "Advancements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdvancementRefunds_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundDetails",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: true),
                    Ammount = table.Column<decimal>(nullable: false),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundDetails_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundFiles",
                schema: "app",
                columns: table => new
                {
                    FileId = table.Column<int>(nullable: false),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundFiles", x => new { x.FileId, x.RefundId });
                    table.ForeignKey(
                        name: "FK_RefundFiles_Files_FileId",
                        column: x => x.FileId,
                        principalSchema: "app",
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundFiles_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefundHistories",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    StatusFromId = table.Column<int>(nullable: false),
                    StatusToId = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(maxLength: 400, nullable: true),
                    RefundId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefundHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefundHistories_Refunds_RefundId",
                        column: x => x.RefundId,
                        principalSchema: "app",
                        principalTable: "Refunds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefundHistories_WorkflowStates_StatusFromId",
                        column: x => x.StatusFromId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RefundHistories_WorkflowStates_StatusToId",
                        column: x => x.StatusToId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvancementRefunds_RefundId",
                schema: "app",
                table: "AdvancementRefunds",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundDetails_RefundId",
                schema: "app",
                table: "RefundDetails",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundFiles_RefundId",
                schema: "app",
                table: "RefundFiles",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundHistories_RefundId",
                schema: "app",
                table: "RefundHistories",
                column: "RefundId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundHistories_StatusFromId",
                schema: "app",
                table: "RefundHistories",
                column: "StatusFromId");

            migrationBuilder.CreateIndex(
                name: "IX_RefundHistories_StatusToId",
                schema: "app",
                table: "RefundHistories",
                column: "StatusToId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_AuthorizerId",
                schema: "app",
                table: "Refunds",
                column: "AuthorizerId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_CurrencyId",
                schema: "app",
                table: "Refunds",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_StatusId",
                schema: "app",
                table: "Refunds",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Refunds_UserApplicantId",
                schema: "app",
                table: "Refunds",
                column: "UserApplicantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvancementRefunds",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefundDetails",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefundFiles",
                schema: "app");

            migrationBuilder.DropTable(
                name: "RefundHistories",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Refunds",
                schema: "app");
        }
    }
}
