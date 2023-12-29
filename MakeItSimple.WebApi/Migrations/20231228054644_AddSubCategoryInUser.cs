using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategoryInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 28, 13, 46, 43, 563, DateTimeKind.Local).AddTicks(8873));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "sub_unit_id" },
                values: new object[] { new DateTime(2023, 12, 28, 13, 46, 43, 563, DateTimeKind.Local).AddTicks(3665), null });

            migrationBuilder.CreateIndex(
                name: "ix_users_sub_unit_id",
                table: "users",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_sub_units_sub_unit_id",
                table: "users",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_sub_units_sub_unit_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_sub_unit_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "sub_unit_id",
                table: "users");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 28, 13, 29, 37, 994, DateTimeKind.Local).AddTicks(7304));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "created_at",
                value: new DateTime(2023, 12, 28, 13, 29, 37, 994, DateTimeKind.Local).AddTicks(2215));
        }
    }
}
