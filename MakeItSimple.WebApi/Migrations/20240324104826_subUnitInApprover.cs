using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class subUnitInApprover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_sub_unit_id",
                table: "approver_ticketings",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_sub_units_sub_unit_id",
                table: "approver_ticketings",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_sub_units_sub_unit_id",
                table: "approver_ticketings");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_sub_unit_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "approver_ticketings");
        }
    }
}
