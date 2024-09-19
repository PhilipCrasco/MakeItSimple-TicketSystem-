using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketConcernInHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_concern_id",
                table: "ticket_histories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_ticket_concern_id",
                table: "ticket_histories",
                column: "ticket_concern_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_ticket_concerns_ticket_concern_id",
                table: "ticket_histories",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_ticket_concerns_ticket_concern_id",
                table: "ticket_histories");

            migrationBuilder.DropIndex(
                name: "ix_ticket_histories_ticket_concern_id",
                table: "ticket_histories");

            migrationBuilder.DropColumn(
                name: "ticket_concern_id",
                table: "ticket_histories");
        }
    }
}
