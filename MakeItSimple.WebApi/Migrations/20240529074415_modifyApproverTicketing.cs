using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class modifyApproverTicketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "issue_handler",
                table: "approver_ticketings");

            migrationBuilder.AddColumn<int>(
                name: "re_ticket_concern_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_re_date_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "transfer_ticket_concern_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_re_ticket_concern_id",
                table: "approver_ticketings",
                column: "re_ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_re_date_id",
                table: "approver_ticketings",
                column: "ticket_re_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_transfer_ticket_concern_id",
                table: "approver_ticketings",
                column: "transfer_ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_re_ticket_concerns_re_ticket_concern_id",
                table: "approver_ticketings",
                column: "re_ticket_concern_id",
                principalTable: "re_ticket_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_re_dates_ticket_re_date_id",
                table: "approver_ticketings",
                column: "ticket_re_date_id",
                principalTable: "ticket_re_dates",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_transfer_ticket_concerns_transfer_ticket_concern_id",
                table: "approver_ticketings",
                column: "transfer_ticket_concern_id",
                principalTable: "transfer_ticket_concerns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_re_ticket_concerns_re_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_re_dates_ticket_re_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_transfer_ticket_concerns_transfer_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_re_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_ticket_re_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_transfer_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "re_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "ticket_re_date_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "transfer_ticket_concern_id",
                table: "approver_ticketings");

            migrationBuilder.AddColumn<Guid>(
                name: "issue_handler",
                table: "approver_ticketings",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
