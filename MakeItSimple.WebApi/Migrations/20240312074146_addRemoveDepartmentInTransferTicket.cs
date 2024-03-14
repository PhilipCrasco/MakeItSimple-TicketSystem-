using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addRemoveDepartmentInTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_business_units_business_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_categories_category_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_companies_company_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_departments_department_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_sub_units_sub_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_units_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_business_units_business_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_categories_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_companies_company_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_departments_department_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_sub_units_sub_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_units_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_business_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_company_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_department_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_sub_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_business_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_company_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_department_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_sub_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_categories_category_id",
                table: "re_ticket_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                table: "re_ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_categories_category_id",
                table: "transfer_ticket_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                table: "transfer_ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_categories_category_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_categories_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "sub_category_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "category_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_business_unit_id",
                table: "transfer_ticket_concerns",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_company_id",
                table: "transfer_ticket_concerns",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_department_id",
                table: "transfer_ticket_concerns",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_sub_unit_id",
                table: "transfer_ticket_concerns",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_unit_id",
                table: "transfer_ticket_concerns",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_business_unit_id",
                table: "re_ticket_concerns",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_company_id",
                table: "re_ticket_concerns",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_department_id",
                table: "re_ticket_concerns",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_sub_unit_id",
                table: "re_ticket_concerns",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_unit_id",
                table: "re_ticket_concerns",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_business_units_business_unit_id",
                table: "re_ticket_concerns",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_categories_category_id",
                table: "re_ticket_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_companies_company_id",
                table: "re_ticket_concerns",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_departments_department_id",
                table: "re_ticket_concerns",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                table: "re_ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_sub_units_sub_unit_id",
                table: "re_ticket_concerns",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_units_unit_id",
                table: "re_ticket_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_business_units_business_unit_id",
                table: "transfer_ticket_concerns",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_categories_category_id",
                table: "transfer_ticket_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_companies_company_id",
                table: "transfer_ticket_concerns",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_departments_department_id",
                table: "transfer_ticket_concerns",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                table: "transfer_ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_sub_units_sub_unit_id",
                table: "transfer_ticket_concerns",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_units_unit_id",
                table: "transfer_ticket_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");
        }
    }
}
