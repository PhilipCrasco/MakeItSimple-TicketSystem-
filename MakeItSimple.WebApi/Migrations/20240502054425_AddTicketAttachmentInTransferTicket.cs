using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAttachmentInTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_generator_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_generator_id",
                table: "ticket_attachments",
                column: "ticket_generator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_generators_ticket_generator_id",
                table: "ticket_attachments",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_generators_ticket_generator_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_ticket_generator_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "ticket_generator_id",
                table: "ticket_attachments");
        }
    }
}
