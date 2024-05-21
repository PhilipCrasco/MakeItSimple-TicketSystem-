using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRequestTransaction : Migration
    {
        /// <inheritdocs />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_request_generators_request_generator_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_request_generators_request_generator_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_request_generators_request_generator_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_request_generators_request_generator_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_request_generators_request_generator_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_request_generators_request_generator_id",
                table: "ticket_comment_views");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comments_request_generators_request_generator_id",
                table: "ticket_comments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_request_generators_request_generator_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_request_generators_request_generator_id",
                table: "ticket_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_request_generators_request_generato",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "request_generators");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "transfer_ticket_concerns",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_transfer_ticket_concerns_request_generator_id",
                table: "transfer_ticket_concerns",
                newName: "ix_transfer_ticket_concerns_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "ticket_histories",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_histories_request_generator_id",
                table: "ticket_histories",
                newName: "ix_ticket_histories_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "ticket_concerns",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_concerns_request_generator_id",
                table: "ticket_concerns",
                newName: "ix_ticket_concerns_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "ticket_comments",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_comments_request_generator_id",
                table: "ticket_comments",
                newName: "ix_ticket_comments_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "ticket_comment_views",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_comment_views_request_generator_id",
                table: "ticket_comment_views",
                newName: "ix_ticket_comment_views_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "ticket_attachments",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_attachments_request_generator_id",
                table: "ticket_attachments",
                newName: "ix_ticket_attachments_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "request_concerns",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_request_concerns_request_generator_id",
                table: "request_concerns",
                newName: "ix_request_concerns_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "re_ticket_concerns",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_re_ticket_concerns_request_generator_id",
                table: "re_ticket_concerns",
                newName: "ix_re_ticket_concerns_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "closing_tickets",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_closing_tickets_request_generator_id",
                table: "closing_tickets",
                newName: "ix_closing_tickets_request_transaction_id");

            migrationBuilder.RenameColumn(
                name: "request_generator_id",
                table: "approver_ticketings",
                newName: "request_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_approver_ticketings_request_generator_id",
                table: "approver_ticketings",
                newName: "ix_approver_ticketings_request_transaction_id");

            migrationBuilder.CreateTable(
                name: "request_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_transactions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_request_transactions_request_transaction",
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
                name: "fk_re_ticket_concerns_request_transactions_request_transaction_",
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
                name: "fk_ticket_attachments_request_transactions_request_transaction_",
                table: "ticket_attachments",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_request_transactions_request_transactio",
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
                name: "fk_ticket_histories_request_transactions_request_transaction_id",
                table: "ticket_histories",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_request_transactions_request_transa",
                table: "transfer_ticket_concerns",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_request_transactions_request_transaction",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_request_transactions_request_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_request_transactions_request_transaction_",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_request_transactions_request_transaction_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_request_transactions_request_transaction_",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_request_transactions_request_transactio",
                table: "ticket_comment_views");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comments_request_transactions_request_transaction_id",
                table: "ticket_comments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_request_transactions_request_transaction_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_request_transactions_request_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_request_transactions_request_transa",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "request_transactions");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "transfer_ticket_concerns",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_transfer_ticket_concerns_request_transaction_id",
                table: "transfer_ticket_concerns",
                newName: "ix_transfer_ticket_concerns_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "ticket_histories",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_histories_request_transaction_id",
                table: "ticket_histories",
                newName: "ix_ticket_histories_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "ticket_concerns",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_concerns_request_transaction_id",
                table: "ticket_concerns",
                newName: "ix_ticket_concerns_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "ticket_comments",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_comments_request_transaction_id",
                table: "ticket_comments",
                newName: "ix_ticket_comments_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "ticket_comment_views",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_comment_views_request_transaction_id",
                table: "ticket_comment_views",
                newName: "ix_ticket_comment_views_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "ticket_attachments",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_attachments_request_transaction_id",
                table: "ticket_attachments",
                newName: "ix_ticket_attachments_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "request_concerns",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_request_concerns_request_transaction_id",
                table: "request_concerns",
                newName: "ix_request_concerns_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "re_ticket_concerns",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_re_ticket_concerns_request_transaction_id",
                table: "re_ticket_concerns",
                newName: "ix_re_ticket_concerns_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "closing_tickets",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_closing_tickets_request_transaction_id",
                table: "closing_tickets",
                newName: "ix_closing_tickets_request_generator_id");

            migrationBuilder.RenameColumn(
                name: "request_transaction_id",
                table: "approver_ticketings",
                newName: "request_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_approver_ticketings_request_transaction_id",
                table: "approver_ticketings",
                newName: "ix_approver_ticketings_request_generator_id");

            migrationBuilder.CreateTable(
                name: "request_generators",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_generators", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_request_generators_request_generator_id",
                table: "approver_ticketings",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_request_generators_request_generator_id",
                table: "closing_tickets",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_request_generators_request_generator_id",
                table: "re_ticket_concerns",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_request_generators_request_generator_id",
                table: "request_concerns",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_request_generators_request_generator_id",
                table: "ticket_attachments",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_request_generators_request_generator_id",
                table: "ticket_comment_views",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comments_request_generators_request_generator_id",
                table: "ticket_comments",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_request_generators_request_generator_id",
                table: "ticket_concerns",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_request_generators_request_generator_id",
                table: "ticket_histories",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_request_generators_request_generato",
                table: "transfer_ticket_concerns",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");
        }
    }
}
