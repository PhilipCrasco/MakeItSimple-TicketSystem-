using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "sync_status",
                table: "departments",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 20, 15, 35, 52, 106, DateTimeKind.Local).AddTicks(3862));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 20, 15, 35, 51, 787, DateTimeKind.Local).AddTicks(6561), "$2a$11$1jp53l8WuSdZvieVzcX3t.GHIUzVaIdu58.xCWTu0Oh19hqPwkxnC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "sync_status",
                table: "departments",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 20, 14, 19, 12, 39, DateTimeKind.Local).AddTicks(8831));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 20, 14, 19, 11, 766, DateTimeKind.Local).AddTicks(1403), "$2a$11$HuVVKjACc3U0kqqwP1uVVupn3F3HuT46ougsa5iTw5fkmMRvZOm5a" });
        }
    }
}
