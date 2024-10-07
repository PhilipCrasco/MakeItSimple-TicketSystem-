using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addcategoryAndSubCategoryInClosingTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_category_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_category_id",
                table: "closing_tickets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_categories_category_id",
                table: "closing_tickets",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_categories_category_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_category_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_sub_category_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "sub_category_id",
                table: "closing_tickets");
        }
    }
}
