using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketConcernInAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_concern_id",
                table: "ticket_attachments",
                column: "ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_concerns_ticket_concern_id",
                table: "ticket_attachments",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_concerns_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_ticket_concern_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "ticket_attachments");
        }
    }
}
