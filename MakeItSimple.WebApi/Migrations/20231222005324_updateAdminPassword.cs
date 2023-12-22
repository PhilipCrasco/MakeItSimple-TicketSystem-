using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateAdminPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 22, 8, 53, 23, 879, DateTimeKind.Local).AddTicks(8461));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 22, 8, 53, 23, 613, DateTimeKind.Local).AddTicks(4238), "$2a$11$RFEIIgj9ylbS1ksOS7YwQOgb.aQzgVsFaNskyJ8QB5xLoeyX/H8j2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
