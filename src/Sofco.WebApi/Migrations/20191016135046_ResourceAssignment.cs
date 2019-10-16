using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sofco.WebApi.Migrations
{
    public partial class ResourceAssignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResourceAssignment",
                schema: "app",
                table: "JobSearchs",
                newName: "ResourceAssignmentId");

            migrationBuilder.CreateTable(
                name: "ResourceAssignments",
                schema: "app",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Text = table.Column<string>(maxLength: 75, nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceAssignments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobSearchs_ResourceAssignmentId",
                schema: "app",
                table: "JobSearchs",
                column: "ResourceAssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobSearchs_ResourceAssignments_ResourceAssignmentId",
                schema: "app",
                table: "JobSearchs",
                column: "ResourceAssignmentId",
                principalSchema: "app",
                principalTable: "ResourceAssignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobSearchs_ResourceAssignments_ResourceAssignmentId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.DropTable(
                name: "ResourceAssignments",
                schema: "app");

            migrationBuilder.DropIndex(
                name: "IX_JobSearchs_ResourceAssignmentId",
                schema: "app",
                table: "JobSearchs");

            migrationBuilder.RenameColumn(
                name: "ResourceAssignmentId",
                schema: "app",
                table: "JobSearchs",
                newName: "ResourceAssignment");
        }
    }
}
