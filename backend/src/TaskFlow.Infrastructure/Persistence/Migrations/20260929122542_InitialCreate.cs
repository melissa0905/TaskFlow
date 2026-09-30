using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "taskflow");

            migrationBuilder.CreateTable(
                name: "projects",
                schema: "taskflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "team_members",
                schema: "taskflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FullName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_team_members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "work_tasks",
                schema: "taskflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedToId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_work_tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_work_tasks_projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "taskflow",
                        principalTable: "projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_work_tasks_team_members_AssignedToId",
                        column: x => x.AssignedToId,
                        principalSchema: "taskflow",
                        principalTable: "team_members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_team_members_Email",
                schema: "taskflow",
                table: "team_members",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_work_tasks_AssignedToId",
                schema: "taskflow",
                table: "work_tasks",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_work_tasks_DueDate",
                schema: "taskflow",
                table: "work_tasks",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_work_tasks_ProjectId_Status",
                schema: "taskflow",
                table: "work_tasks",
                columns: new[] { "ProjectId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "work_tasks",
                schema: "taskflow");

            migrationBuilder.DropTable(
                name: "projects",
                schema: "taskflow");

            migrationBuilder.DropTable(
                name: "team_members",
                schema: "taskflow");
        }
    }
}
