using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class backSubUnitInChannels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "channels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_channels_sub_unit_id",
                table: "channels",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_sub_units_sub_unit_id",
                table: "channels",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_channels_sub_units_sub_unit_id",
                table: "channels");

            migrationBuilder.DropIndex(
                name: "ix_channels_sub_unit_id",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "channels");
        }
    }
}
