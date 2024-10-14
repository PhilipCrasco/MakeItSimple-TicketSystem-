using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addAttachementTicketOnHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_on_hold_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_on_hold_id",
                table: "ticket_attachments",
                column: "ticket_on_hold_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_on_holds_ticket_on_hold_id",
                table: "ticket_attachments",
                column: "ticket_on_hold_id",
                principalTable: "ticket_on_holds",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_on_holds_ticket_on_hold_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_ticket_on_hold_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "ticket_on_hold_id",
                table: "ticket_attachments");
        }
    }
}
