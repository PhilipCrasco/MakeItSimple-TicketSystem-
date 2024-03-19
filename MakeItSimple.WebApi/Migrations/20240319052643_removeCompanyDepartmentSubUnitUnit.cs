using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeCompanyDepartmentSubUnitUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_companies_company_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_departments_department_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_sub_units_sub_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_units_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_company_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_department_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_sub_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "ticket_concerns");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_company_id",
                table: "ticket_concerns",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_department_id",
                table: "ticket_concerns",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_sub_unit_id",
                table: "ticket_concerns",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_unit_id",
                table: "ticket_concerns",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_companies_company_id",
                table: "ticket_concerns",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_departments_department_id",
                table: "ticket_concerns",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_sub_units_sub_unit_id",
                table: "ticket_concerns",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_units_unit_id",
                table: "ticket_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");
        }
    }
}
