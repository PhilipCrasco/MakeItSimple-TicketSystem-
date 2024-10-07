using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addLocationInRequestConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "note",
                table: "closing_tickets",
                newName: "notes");

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "ticket_attachments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "notes",
                table: "request_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_location_id",
                table: "request_concerns",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_locations_location_id",
                table: "request_concerns",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_locations_location_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_location_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "ticket_attachments");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "notes",
                table: "request_concerns");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "closing_tickets",
                newName: "note");
        }
    }
}
