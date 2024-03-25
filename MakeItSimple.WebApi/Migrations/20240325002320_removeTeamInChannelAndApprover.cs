using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeTeamInChannelAndApprover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_teams_team_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_team_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "approvers");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "approver_ticketings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "receiver_id",
                table: "approvers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_team_id",
                table: "approver_ticketings",
                column: "team_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_teams_team_id",
                table: "approver_ticketings",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");
        }
    }
}
