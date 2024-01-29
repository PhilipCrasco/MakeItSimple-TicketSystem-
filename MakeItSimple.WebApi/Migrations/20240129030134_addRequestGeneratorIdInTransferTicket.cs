using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addRequestGeneratorIdInTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "request_generator_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_request_generator_id",
                table: "transfer_ticket_concerns",
                column: "request_generator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_request_generators_request_generato",
                table: "transfer_ticket_concerns",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_request_generators_request_generato",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_request_generator_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "request_generator_id",
                table: "transfer_ticket_concerns");
        }
    }
}
