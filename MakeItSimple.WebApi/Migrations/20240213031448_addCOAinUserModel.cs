using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addCOAinUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "company_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "account_title_id", "company_id", "location_id" },
                values: new object[] { null, null, null });


            migrationBuilder.CreateIndex(
                name: "ix_users_company_id",
                table: "users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_location_id",
                table: "users",
                column: "location_id");


            migrationBuilder.AddForeignKey(
                name: "fk_users_companies_company_id",
                table: "users",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_locations_location_id",
                table: "users",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "fk_users_companies_company_id",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "fk_users_locations_location_id",
                table: "users");


            migrationBuilder.DropIndex(
                name: "ix_users_company_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_location_id",
                table: "users");


            migrationBuilder.DropColumn(
                name: "company_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "users");
        }
    }
}
