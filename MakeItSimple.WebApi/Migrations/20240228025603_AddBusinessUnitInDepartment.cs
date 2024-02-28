using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessUnitInDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "departments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_departments_business_unit_id",
                table: "departments",
                column: "business_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_departments_business_units_business_unit_id",
                table: "departments",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_departments_business_units_business_unit_id",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "ix_departments_business_unit_id",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "departments");
        }
    }
}
