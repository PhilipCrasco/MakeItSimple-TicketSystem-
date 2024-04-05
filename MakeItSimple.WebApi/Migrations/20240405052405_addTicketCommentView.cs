using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTicketCommentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ticket_comment_views",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    is_clicked = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ticket_comment_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_comment_views", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_comment_views_ticket_comments_ticket_comment_id",
                        column: x => x.ticket_comment_id,
                        principalTable: "ticket_comments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_comment_views_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_ticket_comment_id",
                table: "ticket_comment_views",
                column: "ticket_comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_user_id",
                table: "ticket_comment_views",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_comment_views");
        }
    }
}
