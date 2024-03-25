using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeTeamInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_teams_team_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_team_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "team_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_users_team_id",
                table: "users",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_teams_team_id",
                table: "users",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");
        }
    }
}
