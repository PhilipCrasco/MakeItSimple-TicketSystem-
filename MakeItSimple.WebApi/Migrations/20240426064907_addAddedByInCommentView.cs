using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addAddedByInCommentView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_users_user_id",
                table: "ticket_comment_views");

            migrationBuilder.AddColumn<Guid>(
                name: "added_by",
                table: "ticket_comment_views",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_added_by",
                table: "ticket_comment_views",
                column: "added_by");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_users_user_id",
                table: "ticket_comment_views",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_users_user_id1",
                table: "ticket_comment_views",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_users_user_id",
                table: "ticket_comment_views");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_users_user_id1",
                table: "ticket_comment_views");

            migrationBuilder.DropIndex(
                name: "ix_ticket_comment_views_added_by",
                table: "ticket_comment_views");

            migrationBuilder.DropColumn(
                name: "added_by",
                table: "ticket_comment_views");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_users_user_id",
                table: "ticket_comment_views",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
