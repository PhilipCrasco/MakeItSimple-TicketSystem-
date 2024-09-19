using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removalOfRequestTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_request_transactions_request_transaction_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_request_transactions_request_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_request_transactions_request_transaction_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_request_transactions_request_transaction_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_request_transactions_request_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_request_transactions_request_transaction_id",
                table: "ticket_comment_views");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comments_request_transactions_request_transaction_id",
                table: "ticket_comments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_request_transactions_request_transaction_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_re_dates_request_transactions_request_transaction_id",
                table: "ticket_re_dates");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_request_transactions_request_transaction_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "request_transactions");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_request_transaction_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_re_dates_request_transaction_id",
                table: "ticket_re_dates");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_request_transaction_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_comments_request_transaction_id",
                table: "ticket_comments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_comment_views_request_transaction_id",
                table: "ticket_comment_views");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_request_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_request_transaction_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_request_transaction_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_request_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_request_transaction_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "ticket_re_dates");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "ticket_comments");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "ticket_comment_views");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "approver_ticketings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "ticket_re_dates",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "ticket_comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "ticket_comment_views",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "request_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_transactions", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_request_transaction_id",
                table: "transfer_ticket_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_request_transaction_id",
                table: "ticket_re_dates",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_request_transaction_id",
                table: "ticket_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comments_request_transaction_id",
                table: "ticket_comments",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_request_transaction_id",
                table: "ticket_comment_views",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_request_transaction_id",
                table: "ticket_attachments",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_request_transaction_id",
                table: "request_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_request_transaction_id",
                table: "re_ticket_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_request_transaction_id",
                table: "closing_tickets",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_request_transaction_id",
                table: "approver_ticketings",
                column: "request_transaction_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_request_transactions_request_transaction_id",
                table: "approver_ticketings",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_request_transactions_request_transaction_id",
                table: "closing_tickets",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_request_transactions_request_transaction_id",
                table: "re_ticket_concerns",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_request_transactions_request_transaction_id",
                table: "request_concerns",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_request_transactions_request_transaction_id",
                table: "ticket_attachments",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_request_transactions_request_transaction_id",
                table: "ticket_comment_views",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comments_request_transactions_request_transaction_id",
                table: "ticket_comments",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_request_transactions_request_transaction_id",
                table: "ticket_concerns",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_re_dates_request_transactions_request_transaction_id",
                table: "ticket_re_dates",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_request_transactions_request_transaction_id",
                table: "transfer_ticket_concerns",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");
        }
    }
}
