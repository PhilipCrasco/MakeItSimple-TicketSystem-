using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateRequestTypeAndContactNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "contact_number",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "request_type",
                table: "request_concerns",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "contact_number",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "request_type",
                table: "request_concerns");
        }
    }
}
