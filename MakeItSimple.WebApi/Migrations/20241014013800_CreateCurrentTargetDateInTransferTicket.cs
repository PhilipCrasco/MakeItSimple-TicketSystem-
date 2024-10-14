using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateCurrentTargetDateInTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "current_target_date",
                table: "transfer_ticket_concerns",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_target_date",
                table: "transfer_ticket_concerns");
        }
    }
}
