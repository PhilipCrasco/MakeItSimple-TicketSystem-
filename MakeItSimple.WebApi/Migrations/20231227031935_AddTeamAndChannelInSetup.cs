using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndChannelInSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 27, 11, 19, 34, 956, DateTimeKind.Local).AddTicks(3748));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 19, 34, 679, DateTimeKind.Local).AddTicks(7661), "$2a$11$5A9WA9Xfuamgn7qzJfmr9eewtkukKT3.eCaNRt1hn8kOKu7jVaNzi" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 22, 14, 58, 7, 2, DateTimeKind.Local).AddTicks(4703));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 22, 14, 58, 6, 726, DateTimeKind.Local).AddTicks(7562), "$2a$11$43DeuVy1uKJsUNUoEI6y6eAl0qCjfq6Oz/ei16/eaM0d1V3ahMg.q" });
        }
    }
}
