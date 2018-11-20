using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Sofco.WebApi.Migrations
{
    public partial class Workflow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSources",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    SourceId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStates",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStates_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStates_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowTypes",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowTypes_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowTypes_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 200, nullable: true),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    Version = table.Column<string>(maxLength: 50, nullable: true),
                    WorkflowTypeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workflows_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workflows_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Workflows_WorkflowTypes_WorkflowTypeId",
                        column: x => x.WorkflowTypeId,
                        principalSchema: "app",
                        principalTable: "WorkflowTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowReadAccesses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false),
                    WorkflowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowReadAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_UserSources_UserSourceId",
                        column: x => x.UserSourceId,
                        principalSchema: "app",
                        principalTable: "UserSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowReadAccesses_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "app",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStateTransitions",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActualWorkflowStateId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    NextWorkflowStateId = table.Column<int>(nullable: false),
                    NotificationCode = table.Column<string>(maxLength: 50, nullable: true),
                    ValidationCode = table.Column<string>(maxLength: 50, nullable: true),
                    WorkflowId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStateTransitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_WorkflowStates_ActualWorkflowStateId",
                        column: x => x.ActualWorkflowStateId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_WorkflowStates_NextWorkflowStateId",
                        column: x => x.NextWorkflowStateId,
                        principalSchema: "app",
                        principalTable: "WorkflowStates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateTransitions_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalSchema: "app",
                        principalTable: "Workflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStateAccesses",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false),
                    WorkflowStateTransitionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStateAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_UserSources_UserSourceId",
                        column: x => x.UserSourceId,
                        principalSchema: "app",
                        principalTable: "UserSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateAccesses_WorkflowStateTransitions_WorkflowStateTransitionId",
                        column: x => x.WorkflowStateTransitionId,
                        principalSchema: "app",
                        principalTable: "WorkflowStateTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowStateNotifiers",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false),
                    ModifiedAt = table.Column<DateTime>(nullable: false),
                    ModifiedById = table.Column<int>(nullable: false),
                    UserSourceId = table.Column<int>(nullable: false),
                    WorkflowStateTransitionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowStateNotifiers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_Users_ModifiedById",
                        column: x => x.ModifiedById,
                        principalSchema: "app",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_UserSources_UserSourceId",
                        column: x => x.UserSourceId,
                        principalSchema: "app",
                        principalTable: "UserSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowStateNotifiers_WorkflowStateTransitions_WorkflowStateTransitionId",
                        column: x => x.WorkflowStateTransitionId,
                        principalSchema: "app",
                        principalTable: "WorkflowStateTransitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_CreatedById",
                schema: "app",
                table: "Workflows",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ModifiedById",
                schema: "app",
                table: "Workflows",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_WorkflowTypeId",
                schema: "app",
                table: "Workflows",
                column: "WorkflowTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_CreatedById",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_ModifiedById",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_UserSourceId",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "UserSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowReadAccesses_WorkflowId",
                schema: "app",
                table: "WorkflowReadAccesses",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStates_CreatedById",
                schema: "app",
                table: "WorkflowStates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStates_ModifiedById",
                schema: "app",
                table: "WorkflowStates",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_CreatedById",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_ModifiedById",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_UserSourceId",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "UserSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateAccesses_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateAccesses",
                column: "WorkflowStateTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_CreatedById",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_ModifiedById",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_UserSourceId",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "UserSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateNotifiers_WorkflowStateTransitionId",
                schema: "app",
                table: "WorkflowStateNotifiers",
                column: "WorkflowStateTransitionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_ActualWorkflowStateId",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "ActualWorkflowStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_CreatedById",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_ModifiedById",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "ModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_NextWorkflowStateId",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "NextWorkflowStateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowStateTransitions_WorkflowId",
                schema: "app",
                table: "WorkflowStateTransitions",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTypes_CreatedById",
                schema: "app",
                table: "WorkflowTypes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowTypes_ModifiedById",
                schema: "app",
                table: "WorkflowTypes",
                column: "ModifiedById");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkflowReadAccesses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStateAccesses",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStateNotifiers",
                schema: "app");

            migrationBuilder.DropTable(
                name: "UserSources",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStateTransitions",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowStates",
                schema: "app");

            migrationBuilder.DropTable(
                name: "Workflows",
                schema: "app");

            migrationBuilder.DropTable(
                name: "WorkflowTypes",
                schema: "app");
        }
    }
}
