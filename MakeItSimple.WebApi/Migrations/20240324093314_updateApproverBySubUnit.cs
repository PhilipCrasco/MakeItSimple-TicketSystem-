using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateApproverBySubUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "approvers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_approvers_sub_unit_id",
                table: "approvers",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_sub_units_sub_unit_id",
                table: "approvers",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approvers_sub_units_sub_unit_id",
                table: "approvers");

            migrationBuilder.DropIndex(
                name: "ix_approvers_sub_unit_id",
                table: "approvers");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "approvers");
        }
    }
}
