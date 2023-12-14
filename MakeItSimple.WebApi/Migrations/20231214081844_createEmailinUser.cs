using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createEmailinUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("247aaa0f-aaa9-4b46-9839-8426b8c9c19f"));

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 14, 16, 18, 44, 486, DateTimeKind.Local).AddTicks(9284));

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "added_by", "created_at", "email", "fullname", "is_active", "modified_by", "password", "updated_at", "user_role_id", "username" },
                values: new object[] { new Guid("b22c048a-27c5-47a7-a6da-839a4e0f3c63"), null, new DateTime(2023, 12, 14, 16, 18, 44, 218, DateTimeKind.Local).AddTicks(5635), null, "Admin", true, null, "$2a$11$HIcdLGxZp2YAJmnWjMfHRuwaJNZyDgrMeiP5oBmVVGONIHosqtWom", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("b22c048a-27c5-47a7-a6da-839a4e0f3c63"));

            migrationBuilder.DropColumn(
                name: "email",
                table: "users");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 14, 16, 0, 35, 19, DateTimeKind.Local).AddTicks(691));

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "added_by", "created_at", "fullname", "is_active", "modified_by", "password", "updated_at", "user_role_id", "username" },
                values: new object[] { new Guid("247aaa0f-aaa9-4b46-9839-8426b8c9c19f"), null, new DateTime(2023, 12, 14, 16, 0, 34, 746, DateTimeKind.Local).AddTicks(6834), "Admin", true, null, "$2a$11$ceebJ6k5IaBmCYUfLBQE.uKsZBnNZ/q9H1mckaYzLwX5f1oCPuH.W", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "admin" });
        }
    }
}
