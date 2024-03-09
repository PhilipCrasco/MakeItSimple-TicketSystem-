using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketGenerator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ticket_generator_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ticket_generator",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_generator", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_ticket_generator_id",
                table: "closing_tickets",
                column: "ticket_generator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_ticket_generator_ticket_generator_id",
                table: "closing_tickets",
                column: "ticket_generator_id",
                principalTable: "ticket_generator",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_ticket_generator_ticket_generator_id",
                table: "closing_tickets");

            migrationBuilder.DropTable(
                name: "ticket_generator");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_ticket_generator_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "ticket_generator_id",
                table: "closing_tickets");
        }
    }
}
