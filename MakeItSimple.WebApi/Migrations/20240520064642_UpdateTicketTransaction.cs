using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicketTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_generators_ticket_generator_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_ticket_generators_ticket_generator_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_ticket_generators_ticket_generator_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_generators_ticket_generator_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_ticket_generators_ticket_generator_id",
                table: "ticket_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_generators_ticket_generator_",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "ticket_generators");

            migrationBuilder.RenameColumn(
                name: "ticket_generator_id",
                table: "transfer_ticket_concerns",
                newName: "ticket_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_transfer_ticket_concerns_ticket_generator_id",
                table: "transfer_ticket_concerns",
                newName: "ix_transfer_ticket_concerns_ticket_transaction_id");

            migrationBuilder.RenameColumn(
                name: "ticket_generator_id",
                table: "ticket_histories",
                newName: "ticket_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_histories_ticket_generator_id",
                table: "ticket_histories",
                newName: "ix_ticket_histories_ticket_transaction_id");

            migrationBuilder.RenameColumn(
                name: "ticket_generator_id",
                table: "ticket_attachments",
                newName: "ticket_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_attachments_ticket_generator_id",
                table: "ticket_attachments",
                newName: "ix_ticket_attachments_ticket_transaction_id");

            migrationBuilder.RenameColumn(
                name: "ticket_generator_id",
                table: "re_ticket_concerns",
                newName: "ticket_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_re_ticket_concerns_ticket_generator_id",
                table: "re_ticket_concerns",
                newName: "ix_re_ticket_concerns_ticket_transaction_id");

            migrationBuilder.RenameColumn(
                name: "ticket_generator_id",
                table: "closing_tickets",
                newName: "ticket_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_closing_tickets_ticket_generator_id",
                table: "closing_tickets",
                newName: "ix_closing_tickets_ticket_transaction_id");

            migrationBuilder.RenameColumn(
                name: "ticket_generator_id",
                table: "approver_ticketings",
                newName: "ticket_transaction_id");

            migrationBuilder.RenameIndex(
                name: "ix_approver_ticketings_ticket_generator_id",
                table: "approver_ticketings",
                newName: "ix_approver_ticketings_ticket_transaction_id");

            migrationBuilder.CreateTable(
                name: "ticket_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_transactions", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "fk_re_ticket_concerns_ticket_transactions_ticket_transaction_id",
                table: "re_ticket_concerns",
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
                name: "fk_ticket_histories_ticket_transactions_ticket_transaction_id",
                table: "ticket_histories",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_transactions_ticket_transact",
                table: "transfer_ticket_concerns",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_transactions_ticket_transaction_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_ticket_transactions_ticket_transaction_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_ticket_transactions_ticket_transaction_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_transactions_ticket_transaction_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_ticket_transactions_ticket_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_transactions_ticket_transact",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "ticket_transactions");

            migrationBuilder.RenameColumn(
                name: "ticket_transaction_id",
                table: "transfer_ticket_concerns",
                newName: "ticket_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_transfer_ticket_concerns_ticket_transaction_id",
                table: "transfer_ticket_concerns",
                newName: "ix_transfer_ticket_concerns_ticket_generator_id");

            migrationBuilder.RenameColumn(
                name: "ticket_transaction_id",
                table: "ticket_histories",
                newName: "ticket_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_histories_ticket_transaction_id",
                table: "ticket_histories",
                newName: "ix_ticket_histories_ticket_generator_id");

            migrationBuilder.RenameColumn(
                name: "ticket_transaction_id",
                table: "ticket_attachments",
                newName: "ticket_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_attachments_ticket_transaction_id",
                table: "ticket_attachments",
                newName: "ix_ticket_attachments_ticket_generator_id");

            migrationBuilder.RenameColumn(
                name: "ticket_transaction_id",
                table: "re_ticket_concerns",
                newName: "ticket_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_re_ticket_concerns_ticket_transaction_id",
                table: "re_ticket_concerns",
                newName: "ix_re_ticket_concerns_ticket_generator_id");

            migrationBuilder.RenameColumn(
                name: "ticket_transaction_id",
                table: "closing_tickets",
                newName: "ticket_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_closing_tickets_ticket_transaction_id",
                table: "closing_tickets",
                newName: "ix_closing_tickets_ticket_generator_id");

            migrationBuilder.RenameColumn(
                name: "ticket_transaction_id",
                table: "approver_ticketings",
                newName: "ticket_generator_id");

            migrationBuilder.RenameIndex(
                name: "ix_approver_ticketings_ticket_transaction_id",
                table: "approver_ticketings",
                newName: "ix_approver_ticketings_ticket_generator_id");

            migrationBuilder.CreateTable(
                name: "ticket_generators",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_generators", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_generators_ticket_generator_id",
                table: "approver_ticketings",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_ticket_generators_ticket_generator_id",
                table: "closing_tickets",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_ticket_generators_ticket_generator_id",
                table: "re_ticket_concerns",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_generators_ticket_generator_id",
                table: "ticket_attachments",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_ticket_generators_ticket_generator_id",
                table: "ticket_histories",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_generators_ticket_generator_",
                table: "transfer_ticket_concerns",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");
        }
    }
}
