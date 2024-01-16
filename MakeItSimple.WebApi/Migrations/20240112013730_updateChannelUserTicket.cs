using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateChannelUserTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_channel_users_channel_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_added_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_channel_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "channel_user_id",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "ticket_concerns",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_user_id",
                table: "ticket_concerns",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id",
                table: "ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<int>(
                name: "channel_user_id",
                table: "ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_channel_user_id",
                table: "ticket_concerns",
                column: "channel_user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_channel_users_channel_user_id",
                table: "ticket_concerns",
                column: "channel_user_id",
                principalTable: "channel_users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_added_by_user_id",
                table: "ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
