using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createTicketOnHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_on_holds",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_hold = table.Column<bool>(type: "bit", nullable: true),
                    resume_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_on_holds", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_on_holds_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_on_holds_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_ticket_on_holds_added_by",
                table: "ticket_on_holds",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_on_holds_ticket_concern_id",
                table: "ticket_on_holds",
                column: "ticket_concern_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_on_holds");
        }
    }
}
