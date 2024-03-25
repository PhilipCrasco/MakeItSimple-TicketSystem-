using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeTeamInApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approvers_teams_team_id",
                table: "approvers");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_teams_team_id",
                table: "channels");

            migrationBuilder.DropIndex(
                name: "ix_channels_team_id",
                table: "channels");

            migrationBuilder.DropIndex(
                name: "ix_approvers_team_id",
                table: "approvers");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "approvers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "approvers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_channels_team_id",
                table: "channels",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_team_id",
                table: "approvers",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_teams_team_id",
                table: "approvers",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_teams_team_id",
                table: "channels",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");
        }
    }
}
