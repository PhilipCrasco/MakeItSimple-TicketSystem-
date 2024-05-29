using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketConcernInApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_concern_id",
                table: "approver_ticketings",
                column: "ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_concerns_ticket_concern_id",
                table: "approver_ticketings",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_concerns_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "approver_ticketings");
        }
    }
}
