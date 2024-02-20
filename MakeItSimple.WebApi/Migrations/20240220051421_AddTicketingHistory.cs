using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketingHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    request_generator_id = table.Column<int>(type: "int", nullable: true),
                    requestor_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    approver_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    request = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    transaction_date = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_histories_request_generators_request_generator_id",
                        column: x => x.request_generator_id,
                        principalTable: "request_generators",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_histories_users_approver_by_user_id",
                        column: x => x.approver_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ticket_histories_users_requestor_by_user_id",
                        column: x => x.requestor_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_approver_by",
                table: "ticket_histories",
                column: "approver_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_request_generator_id",
                table: "ticket_histories",
                column: "request_generator_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_requestor_by",
                table: "ticket_histories",
                column: "requestor_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_histories");
        }
    }
}
