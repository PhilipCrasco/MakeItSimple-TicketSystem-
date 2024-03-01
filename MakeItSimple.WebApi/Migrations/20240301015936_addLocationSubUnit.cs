using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addLocationSubUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sub_units_locations_location_id",
                table: "sub_units");

            migrationBuilder.DropIndex(
                name: "ix_sub_units_location_id",
                table: "sub_units");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "sub_units");

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "locations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_locations_sub_unit_id",
                table: "locations",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_locations_sub_units_sub_unit_id",
                table: "locations",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_locations_sub_units_sub_unit_id",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "ix_locations_sub_unit_id",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "locations");

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "sub_units",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_location_id",
                table: "sub_units",
                column: "location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_sub_units_locations_location_id",
                table: "sub_units",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");
        }
    }
}
