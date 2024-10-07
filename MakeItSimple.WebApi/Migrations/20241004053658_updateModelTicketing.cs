using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateModelTicketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_categories_category_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_sub_categories_sub_category_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id6",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id7",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_category_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_re_date_by",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_reticket_by",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_sub_category_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "concern_details",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_closed_reject",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_re_date",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_re_ticket",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "is_reject",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "re_date_at",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "re_date_by",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reticket_at",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "reticket_by",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "start_date",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "sub_category_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_approver",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_type",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "ticket_no",
                table: "closing_tickets");

            migrationBuilder.RenameColumn(
                name: "expected_date",
                table: "request_concerns",
                newName: "date_needed");

            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_category_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_business_unit_id",
                table: "request_concerns",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_category_id",
                table: "request_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_company_id",
                table: "request_concerns",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_department_id",
                table: "request_concerns",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_sub_category_id",
                table: "request_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_sub_unit_id",
                table: "request_concerns",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_unit_id",
                table: "request_concerns",
                column: "unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_business_units_business_unit_id",
                table: "request_concerns",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_categories_category_id",
                table: "request_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_companies_company_id",
                table: "request_concerns",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_departments_department_id",
                table: "request_concerns",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_sub_categories_sub_category_id",
                table: "request_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_sub_units_sub_unit_id",
                table: "request_concerns",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_units_unit_id",
                table: "request_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_business_units_business_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_categories_category_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_companies_company_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_departments_department_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_sub_categories_sub_category_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_sub_units_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_units_unit_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_business_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_category_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_company_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_department_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_sub_category_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_unit_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "business_unit_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "company_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "sub_category_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "request_concerns");

            migrationBuilder.RenameColumn(
                name: "date_needed",
                table: "request_concerns",
                newName: "expected_date");

            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "transfer_ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "category_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "concern_details",
                table: "ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_closed_reject",
                table: "ticket_concerns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_re_date",
                table: "ticket_concerns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_re_ticket",
                table: "ticket_concerns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_reject",
                table: "ticket_concerns",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "re_date_at",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "re_date_by",
                table: "ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "reticket_at",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "reticket_by",
                table: "ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "start_date",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "sub_category_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ticket_approver",
                table: "ticket_concerns",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticket_type",
                table: "ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ticket_no",
                table: "closing_tickets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_category_id",
                table: "ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_re_date_by",
                table: "ticket_concerns",
                column: "re_date_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_reticket_by",
                table: "ticket_concerns",
                column: "reticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_sub_category_id",
                table: "ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_categories_category_id",
                table: "ticket_concerns",
                column: "category_id",
                principalTable: "categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_sub_categories_sub_category_id",
                table: "ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns",
                column: "re_date_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id6",
                table: "ticket_concerns",
                column: "reticket_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id7",
                table: "ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
