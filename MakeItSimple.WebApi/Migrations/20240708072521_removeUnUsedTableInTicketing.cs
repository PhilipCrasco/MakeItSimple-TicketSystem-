using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeUnUsedTableInTicketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_sub_units_sub_unit_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_categories_category_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_channels_channel_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_users_user_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_users_user_id1",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_categories_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_channels_channel_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_users_user_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_users_user_id1",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_channel_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_sub_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_user_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_category_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_channel_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_sub_category_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_user_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_channel_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_sub_unit_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "concern_details",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "sub_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "concern_details",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "sub_category_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "target_date",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "approver_ticketings");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_added_by_user_id",
                table: "closing_tickets",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_added_by_user_id",
                table: "transfer_ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_users_added_by_user_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_users_added_by_user_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "concern_details",
                table: "transfer_ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_category_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "transfer_ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "concern_details",
                table: "closing_tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "closing_tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_category_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "target_date",
                table: "closing_tickets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                table: "closing_tickets",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_category_id",
                table: "transfer_ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_channel_id",
                table: "transfer_ticket_concerns",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_sub_category_id",
                table: "transfer_ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_user_id",
                table: "transfer_ticket_concerns",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_category_id",
                table: "closing_tickets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_channel_id",
                table: "closing_tickets",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_user_id",
                table: "closing_tickets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_channel_id",
                table: "approver_ticketings",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_sub_unit_id",
                table: "approver_ticketings",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_sub_units_sub_unit_id",
                table: "approver_ticketings",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_categories_category_id",
                table: "closing_tickets",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_channels_channel_id",
                table: "closing_tickets",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_user_id",
                table: "closing_tickets",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_user_id1",
                table: "closing_tickets",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_categories_category_id",
                table: "transfer_ticket_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_channels_channel_id",
                table: "transfer_ticket_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                table: "transfer_ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_user_id",
                table: "transfer_ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_user_id1",
                table: "transfer_ticket_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
