using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addIsConfirmInRequestTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "confirm_at",
                table: "request_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "is_confirm",
                table: "request_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "resolution",
                table: "request_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_closing_ticket_id",
                table: "ticket_attachments",
                column: "closing_ticket_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_closing_tickets_closing_ticket_id",
                table: "ticket_attachments",
                column: "closing_ticket_id",
                principalTable: "closing_tickets",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_closing_tickets_closing_ticket_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_closing_ticket_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "confirm_at",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "is_confirm",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "resolution",
                table: "request_concerns");
        }
    }
}
