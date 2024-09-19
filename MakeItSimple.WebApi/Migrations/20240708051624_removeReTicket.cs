using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeReTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_re_ticket_concerns_re_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_re_ticket_concerns_re_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropTable(
                name: "re_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_re_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_re_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "re_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "re_ticket_concern_id",
                table: "approver_ticketings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "re_ticket_concern_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "re_ticket_concern_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "re_ticket_concerns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_re_ticket_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    re_ticket_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_category_id = table.Column<int>(type: "int", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: false),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    concern_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    is_re_ticket = table.Column<bool>(type: "bit", nullable: true),
                    is_reject_re_ticket = table.Column<bool>(type: "bit", nullable: false),
                    re_ticket_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    re_ticket_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    reject_re_ticket_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reject_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_re_ticket_concerns", x => x.id);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_re_ticket_by_user_id",
                        column: x => x.re_ticket_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_reject_re_ticket_by_user_id",
                        column: x => x.reject_re_ticket_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_re_ticket_concern_id",
                table: "ticket_attachments",
                column: "re_ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_re_ticket_concern_id",
                table: "approver_ticketings",
                column: "re_ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_added_by",
                table: "re_ticket_concerns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_category_id",
                table: "re_ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_channel_id",
                table: "re_ticket_concerns",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_modified_by",
                table: "re_ticket_concerns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_re_ticket_by",
                table: "re_ticket_concerns",
                column: "re_ticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_reject_re_ticket_by",
                table: "re_ticket_concerns",
                column: "reject_re_ticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_sub_category_id",
                table: "re_ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_ticket_concern_id",
                table: "re_ticket_concerns",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_ticket_transaction_id",
                table: "re_ticket_concerns",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_user_id",
                table: "re_ticket_concerns",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_re_ticket_concerns_re_ticket_concern_id",
                table: "approver_ticketings",
                column: "re_ticket_concern_id",
                principalTable: "re_ticket_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_re_ticket_concerns_re_ticket_concern_id",
                table: "ticket_attachments",
                column: "re_ticket_concern_id",
                principalTable: "re_ticket_concerns",
                principalColumn: "id");
        }
    }
}
