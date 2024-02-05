using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class adjustTicketforApproverHiercy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_reject_transfer",
                table: "transfer_ticket_concerns",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "reject_transfer_at",
                table: "transfer_ticket_concerns",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "reject_transfer_by",
                table: "transfer_ticket_concerns",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<bool>(
                name: "is_re_ticket",
                table: "ticket_concerns",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<Guid>(
                name: "reticket_by",
                table: "ticket_concerns",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateTable(
                name: "approver_ticketings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    channel_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    approver_level = table.Column<int>(type: "int", nullable: true),
                    current_level = table.Column<int>(type: "int", nullable: true),
                    request_generator_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approver_ticketings", x => x.id);
                    table.ForeignKey(
                        name: "fk_approver_ticketings_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_approver_ticketings_request_generators_request_generator_id",
                        column: x => x.request_generator_id,
                        principalTable: "request_generators",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_approver_ticketings_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_approver_ticketings_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_reject_transfer_by",
                table: "transfer_ticket_concerns",
                column: "reject_transfer_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_reticket_by",
                table: "ticket_concerns",
                column: "reticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_added_by",
                table: "approver_ticketings",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_channel_id",
                table: "approver_ticketings",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_request_generator_id",
                table: "approver_ticketings",
                column: "request_generator_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_user_id",
                table: "approver_ticketings",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_reticket_by_user_id",
                table: "ticket_concerns",
                column: "reticket_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_reject_transfer_by_user_id",
                table: "transfer_ticket_concerns",
                column: "reject_transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_reticket_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_users_reject_transfer_by_user_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_reject_transfer_by",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_reticket_by",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_reject_transfer",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reject_transfer_at",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reject_transfer_by",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reticket_by",
                table: "ticket_concerns");

            migrationBuilder.AlterColumn<bool>(
                name: "is_re_ticket",
                table: "ticket_concerns",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);
        }
    }
}
