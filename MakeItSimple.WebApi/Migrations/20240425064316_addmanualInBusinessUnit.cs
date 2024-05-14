using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addmanualInBusinessUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "request_generator_id",
                table: "ticket_comment_views",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "manual",
                table: "sub_units",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_request_generator_id",
                table: "ticket_comment_views",
                column: "request_generator_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_request_generators_request_generator_id",
                table: "ticket_comment_views",
                column: "request_generator_id",
                principalTable: "request_generators",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_comment_views_request_generators_request_generator_id",
                table: "ticket_comment_views");

            migrationBuilder.DropIndex(
                name: "ix_ticket_comment_views_request_generator_id",
                table: "ticket_comment_views");

            migrationBuilder.DropColumn(
                name: "request_generator_id",
                table: "ticket_comment_views");

            migrationBuilder.DropColumn(
                name: "manual",
                table: "sub_units");
        }
    }
}
