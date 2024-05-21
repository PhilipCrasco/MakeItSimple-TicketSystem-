using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeTargetDateInTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "start_date",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "target_date",
                table: "transfer_ticket_concerns");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "transfer_ticket_concerns",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "target_date",
                table: "transfer_ticket_concerns",
                type: "datetime(6)",
                nullable: true);
        }
    }
}
