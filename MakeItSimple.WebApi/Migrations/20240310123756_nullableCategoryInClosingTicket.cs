using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class nullableCategoryInClosingTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_categories_category_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets");

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "closing_tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "closing_tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "closing_tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "closing_tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_categories_category_id",
                table: "closing_tickets",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
