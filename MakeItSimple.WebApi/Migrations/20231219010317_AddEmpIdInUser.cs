using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddEmpIdInUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "emp_id",
                table: "users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 19, 9, 3, 17, 93, DateTimeKind.Local).AddTicks(3206));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "emp_id", "password" },
                values: new object[] { new DateTime(2023, 12, 19, 9, 3, 16, 806, DateTimeKind.Local).AddTicks(757), null, "$2a$11$MyPFNsaGkaMg1nJxjbDADOAPOHiNuz0zoZXA5PXCKQ5F2a8IfgAXC" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "emp_id",
                table: "users");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 18, 9, 32, 44, 711, DateTimeKind.Local).AddTicks(4208));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 18, 9, 32, 44, 437, DateTimeKind.Local).AddTicks(7897), "$2a$11$lGXiYKF276plczW5/ICsT.nUe0IWI5Cid5n4wThkP5gCytwn5Btb." });
        }
    }
}
