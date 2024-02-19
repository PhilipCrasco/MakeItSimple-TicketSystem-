using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addSyncInSubUnitAndLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "sync_date",
                table: "sub_units",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "sync_status",
                table: "sub_units",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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
                name: "sync_date",
                table: "sub_units");

            migrationBuilder.DropColumn(
                name: "sync_status",
                table: "sub_units");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "locations");
        }
    }
}
