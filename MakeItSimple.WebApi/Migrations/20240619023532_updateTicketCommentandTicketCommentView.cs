using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketCommentandTicketCommentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_request_transactions_request_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_ticket_transactions_ticket_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropIndex(
                name: "ix_ticket_histories_request_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropIndex(
                name: "ix_ticket_histories_ticket_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropColumn(
                name: "request_transaction_id",
                table: "ticket_histories");

            migrationBuilder.DropColumn(
                name: "ticket_transaction_id",
                table: "ticket_histories");

            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "ticket_comments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "ticket_comment_views",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comments_ticket_concern_id",
                table: "ticket_comments",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_ticket_concern_id",
                table: "ticket_comment_views",
                column: "ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_ticket_concerns_ticket_concern_id",
                table: "ticket_comment_views",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comments_ticket_concerns_ticket_concern_id",
                table: "ticket_comments",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_ticket_concerns_ticket_concern_id",
                table: "ticket_comment_views");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comments_ticket_concerns_ticket_concern_id",
                table: "ticket_comments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_comments_ticket_concern_id",
                table: "ticket_comments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_comment_views_ticket_concern_id",
                table: "ticket_comment_views");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "ticket_comments");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "ticket_comment_views");

            migrationBuilder.AddColumn<int>(
                name: "request_transaction_id",
                table: "ticket_histories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_transaction_id",
                table: "ticket_histories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_request_transaction_id",
                table: "ticket_histories",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_ticket_transaction_id",
                table: "ticket_histories",
                column: "ticket_transaction_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_request_transactions_request_transaction_id",
                table: "ticket_histories",
                column: "request_transaction_id",
                principalTable: "request_transactions",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_ticket_transactions_ticket_transaction_id",
                table: "ticket_histories",
                column: "ticket_transaction_id",
                principalTable: "ticket_transactions",
                principalColumn: "id");
        }
    }
}
