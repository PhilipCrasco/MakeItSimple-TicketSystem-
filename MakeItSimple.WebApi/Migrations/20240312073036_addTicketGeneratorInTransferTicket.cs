using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketGeneratorInTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_generator_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_generator_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_ticket_generator_id",
                table: "transfer_ticket_concerns",
                column: "ticket_generator_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_ticket_generator_id",
                table: "re_ticket_concerns",
                column: "ticket_generator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_ticket_generators_ticket_generator_id",
                table: "re_ticket_concerns",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_generators_ticket_generator_",
                table: "transfer_ticket_concerns",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_ticket_generators_ticket_generator_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_ticket_generators_ticket_generator_",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_ticket_generator_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_ticket_generator_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_generator_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_generator_id",
                table: "re_ticket_concerns");
        }
    }
}
