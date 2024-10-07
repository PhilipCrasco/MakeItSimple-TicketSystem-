using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTransferToAndNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "transfer_to",
                table: "transfer_ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "closing_tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_transfer_to",
                table: "transfer_ticket_concerns",
                column: "transfer_to");

            migrationBuilder.CreateIndex(
                name: "ix_categories_channel_id",
                table: "categories",
                column: "channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_categories_channels_channel_id",
                table: "categories",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_transfer_to_user_id",
                table: "transfer_ticket_concerns",
                column: "transfer_to",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_categories_channels_channel_id",
                table: "categories");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_users_transfer_to_user_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_transfer_to",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_categories_channel_id",
                table: "categories");

            migrationBuilder.DropColumn(
                name: "transfer_to",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "note",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "categories");
        }
    }
}
