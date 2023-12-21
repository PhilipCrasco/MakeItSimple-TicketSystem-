using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateCompanyNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "company_no",
                table: "companies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 21, 14, 9, 49, 937, DateTimeKind.Local).AddTicks(8016));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 21, 14, 9, 49, 665, DateTimeKind.Local).AddTicks(4374), "$2a$11$ptzhAz286F7WXiQc/txSYOc8hMLIKIoYrimFFt5gWnCxlrRoC4ZGC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "company_no",
                table: "companies");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 21, 14, 2, 30, 141, DateTimeKind.Local).AddTicks(6584));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 21, 14, 2, 29, 859, DateTimeKind.Local).AddTicks(5443), "$2a$11$Qz1JnRT5zyfgts/0aVYZQOp2YYThXt1YuCKN6GptrSthOWJuOyvq." });
        }
    }
}
