using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addClosingTicketIdInClosingTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "closing_ticket_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_closing_ticket_id",
                table: "approver_ticketings",
                column: "closing_ticket_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_closing_tickets_closing_ticket_id",
                table: "approver_ticketings",
                column: "closing_ticket_id",
                principalTable: "closing_tickets",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_closing_tickets_closing_ticket_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_closing_ticket_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "closing_ticket_id",
                table: "approver_ticketings");
        }
    }
}
