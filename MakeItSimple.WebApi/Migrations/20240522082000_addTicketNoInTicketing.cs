using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketNoInTicketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "transfer_ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "re_ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "closing_tickets",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "closing_tickets");
        }
    }
}
