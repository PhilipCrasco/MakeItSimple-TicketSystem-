using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_users_approver_by_user_id",
                table: "ticket_histories");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_users_requestor_by_user_id",
                table: "ticket_histories");

            migrationBuilder.DropIndex(
                name: "ix_ticket_histories_approver_by",
                table: "ticket_histories");

            migrationBuilder.DropColumn(
                name: "approver_by",
                table: "ticket_histories");

            migrationBuilder.RenameColumn(
                name: "requestor_by",
                table: "ticket_histories",
                newName: "transacted_by");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_histories_requestor_by",
                table: "ticket_histories",
                newName: "ix_ticket_histories_transacted_by");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_users_transacted_by_user_id",
                table: "ticket_histories",
                column: "transacted_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_users_transacted_by_user_id",
                table: "ticket_histories");

            migrationBuilder.RenameColumn(
                name: "transacted_by",
                table: "ticket_histories",
                newName: "requestor_by");

            migrationBuilder.RenameIndex(
                name: "ix_ticket_histories_transacted_by",
                table: "ticket_histories",
                newName: "ix_ticket_histories_requestor_by");

            migrationBuilder.AddColumn<Guid>(
                name: "approver_by",
                table: "ticket_histories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_approver_by",
                table: "ticket_histories",
                column: "approver_by");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_users_approver_by_user_id",
                table: "ticket_histories",
                column: "approver_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_users_requestor_by_user_id",
                table: "ticket_histories",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
