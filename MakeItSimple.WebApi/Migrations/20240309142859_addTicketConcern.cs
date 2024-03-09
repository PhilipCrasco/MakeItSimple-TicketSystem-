using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_ticket_generator_ticket_generator_id",
                table: "closing_tickets");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ticket_generator",
                table: "ticket_generator");

            migrationBuilder.RenameTable(
                name: "ticket_generator",
                newName: "ticket_generators");

            migrationBuilder.AddColumn<int>(
                name: "ticket_generator_id",
                table: "ticket_histories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ticket_generator_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "pk_ticket_generators",
                table: "ticket_generators",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_ticket_generator_id",
                table: "ticket_histories",
                column: "ticket_generator_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_generator_id",
                table: "approver_ticketings",
                column: "ticket_generator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_ticket_generators_ticket_generator_id",
                table: "approver_ticketings",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_ticket_generators_ticket_generator_id",
                table: "closing_tickets",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_ticket_generators_ticket_generator_id",
                table: "ticket_histories",
                column: "ticket_generator_id",
                principalTable: "ticket_generators",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_ticket_generators_ticket_generator_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_ticket_generators_ticket_generator_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_histories_ticket_generators_ticket_generator_id",
                table: "ticket_histories");

            migrationBuilder.DropIndex(
                name: "ix_ticket_histories_ticket_generator_id",
                table: "ticket_histories");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_ticket_generator_id",
                table: "approver_ticketings");

            migrationBuilder.DropPrimaryKey(
                name: "pk_ticket_generators",
                table: "ticket_generators");

            migrationBuilder.DropColumn(
                name: "ticket_generator_id",
                table: "ticket_histories");

            migrationBuilder.DropColumn(
                name: "ticket_generator_id",
                table: "approver_ticketings");

            migrationBuilder.RenameTable(
                name: "ticket_generators",
                newName: "ticket_generator");

            migrationBuilder.AddPrimaryKey(
                name: "pk_ticket_generator",
                table: "ticket_generator",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_ticket_generator_ticket_generator_id",
                table: "closing_tickets",
                column: "ticket_generator_id",
                principalTable: "ticket_generator",
                principalColumn: "id");
        }
    }
}
