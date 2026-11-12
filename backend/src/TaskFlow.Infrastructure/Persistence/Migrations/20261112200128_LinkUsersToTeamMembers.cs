using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class LinkUsersToTeamMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TeamMemberId",
                schema: "taskflow",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_TeamMemberId",
                schema: "taskflow",
                table: "users",
                column: "TeamMemberId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_users_team_members_TeamMemberId",
                schema: "taskflow",
                table: "users",
                column: "TeamMemberId",
                principalSchema: "taskflow",
                principalTable: "team_members",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_team_members_TeamMemberId",
                schema: "taskflow",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_TeamMemberId",
                schema: "taskflow",
                table: "users");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                schema: "taskflow",
                table: "users");
        }
    }
}
