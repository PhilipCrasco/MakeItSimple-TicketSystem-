using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeTicketTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_transactions_ticket_transaction_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_ticket_transactions_ticket_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_transactions_ticket_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_transactions_ticket_transaction_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "ticket_transactions");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_ticket_transaction_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_ticket_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_ticket_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_ticket_transaction_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_transaction_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "ticket_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "ticket_transaction_id",
                table: "approver_ticketings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "receiver_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_transaction_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_transaction_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "receiver_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_transaction_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_transaction_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ticket_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_transactions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_ticket_transaction_id",
                table: "transfer_ticket_concerns",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_transaction_id",
                table: "ticket_attachments",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_ticket_transaction_id",
                table: "closing_tickets",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_transaction_id",
                table: "approver_ticketings",
                column: "ticket_transaction_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_transactions_ticket_transaction_id",
                table: "approver_ticketings",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_ticket_transactions_ticket_transaction_id",
                table: "closing_tickets",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_transactions_ticket_transaction_id",
                table: "ticket_attachments",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_transactions_ticket_transaction_id",
                table: "transfer_ticket_concerns",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");
        }
    }
}
