using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addDepartmentInDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_department_id",
                table: "transfer_ticket_concerns",
                column: "department_id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_departments_department_id",
                table: "transfer_ticket_concerns",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_departments_department_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_department_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "transfer_ticket_concerns");
        }
    }
}
