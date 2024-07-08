using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeReDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_re_dates_ticket_re_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_re_dates_ticket_re_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropTable(
                name: "ticket_re_dates");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_ticket_re_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_ticket_re_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "ticket_re_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "ticket_re_date_id",
                table: "approver_ticketings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_re_date_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_re_date_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ticket_re_dates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    re_date_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_re_date_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true),
                    concern_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_re_date = table.Column<bool>(type: "bit", nullable: true),
                    is_reject_re_date = table.Column<bool>(type: "bit", nullable: false),
                    re_date_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    re_date_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    reject_re_date_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reject_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_re_dates", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_re_dates_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_re_dates_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_re_dates_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ticket_re_dates_users_re_date_by_user_id",
                        column: x => x.re_date_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ticket_re_dates_users_reject_re_date_by_user_id",
                        column: x => x.reject_re_date_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ticket_re_dates_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_re_date_id",
                table: "ticket_attachments",
                column: "ticket_re_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_re_date_id",
                table: "approver_ticketings",
                column: "ticket_re_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_added_by",
                table: "ticket_re_dates",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_modified_by",
                table: "ticket_re_dates",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_re_date_by",
                table: "ticket_re_dates",
                column: "re_date_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_reject_re_date_by",
                table: "ticket_re_dates",
                column: "reject_re_date_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_ticket_concern_id",
                table: "ticket_re_dates",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_re_dates_ticket_transaction_id",
                table: "ticket_re_dates",
                column: "ticket_transaction_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_re_dates_ticket_re_date_id",
                table: "approver_ticketings",
                column: "ticket_re_date_id",
                principalTable: "ticket_re_dates",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_re_dates_ticket_re_date_id",
                table: "ticket_attachments",
                column: "ticket_re_date_id",
                principalTable: "ticket_re_dates",
                principalColumn: "id");
        }
    }
}
