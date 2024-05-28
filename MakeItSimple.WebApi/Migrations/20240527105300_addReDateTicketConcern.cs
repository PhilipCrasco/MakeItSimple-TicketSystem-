using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addReDateTicketConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<bool>(
                name: "is_re_date",
                table: "ticket_concerns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "re_date_at",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "re_date_by",
                table: "ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_re_date_by",
                table: "ticket_concerns",
                column: "re_date_by");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_re_date_by_user_id",
                table: "ticket_concerns",
                column: "re_date_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_re_date_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_re_date_by",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_re_date",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "re_date_at",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "re_date_by",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns",
                column: "ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");
        }
    }
}
