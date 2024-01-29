using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRemarksInTicketConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "close_reject_remarks",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reject_remarks",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "transfer_remarks",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<bool>(
                name: "is_re_ticket",
                table: "ticket_concerns",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "reticket_at",
                table: "ticket_concerns",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_re_ticket",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reticket_at",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<string>(
                name: "close_reject_remarks",
                table: "ticket_concerns",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "reject_remarks",
                table: "ticket_concerns",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "transfer_remarks",
                table: "ticket_concerns",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
