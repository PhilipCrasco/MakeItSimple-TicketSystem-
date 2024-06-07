using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateTicketAttachmentByTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "closing_ticket_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "re_ticket_concern_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_re_date_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "transfer_ticket_concern_id",
                table: "ticket_attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_re_ticket_concern_id",
                table: "ticket_attachments",
                column: "re_ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_re_date_id",
                table: "ticket_attachments",
                column: "ticket_re_date_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_transfer_ticket_concern_id",
                table: "ticket_attachments",
                column: "transfer_ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_re_ticket_concerns_re_ticket_concern_id",
                table: "ticket_attachments",
                column: "re_ticket_concern_id",
                principalTable: "re_ticket_concerns",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_ticket_re_dates_ticket_re_date_id",
                table: "ticket_attachments",
                column: "ticket_re_date_id",
                principalTable: "ticket_re_dates",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_transfer_ticket_concerns_transfer_ticket_concern_id",
                table: "ticket_attachments",
                column: "transfer_ticket_concern_id",
                principalTable: "transfer_ticket_concerns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_re_ticket_concerns_re_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_ticket_re_dates_ticket_re_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_attachments_transfer_ticket_concerns_transfer_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_re_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_ticket_re_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropIndex(
                name: "ix_ticket_attachments_transfer_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "closing_ticket_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "re_ticket_concern_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "ticket_re_date_id",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "transfer_ticket_concern_id",
                table: "ticket_attachments");
        }
    }
}
