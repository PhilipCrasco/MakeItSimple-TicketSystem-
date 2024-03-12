using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeDepartmentInCLosingTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_business_units_business_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_companies_company_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_departments_department_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_sub_units_sub_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_units_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_business_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_company_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_department_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_sub_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropIndex(
                name: "ix_closing_tickets_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "closing_tickets");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "closing_tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "closing_tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "closing_tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_business_unit_id",
                table: "closing_tickets",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_company_id",
                table: "closing_tickets",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_department_id",
                table: "closing_tickets",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_sub_unit_id",
                table: "closing_tickets",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_unit_id",
                table: "closing_tickets",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_business_units_business_unit_id",
                table: "closing_tickets",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_companies_company_id",
                table: "closing_tickets",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_departments_department_id",
                table: "closing_tickets",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_sub_units_sub_unit_id",
                table: "closing_tickets",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_units_unit_id",
                table: "closing_tickets",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");
        }
    }
}
