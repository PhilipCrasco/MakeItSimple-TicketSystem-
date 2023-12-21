using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Department : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "department_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    modified_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    department_no = table.Column<int>(type: "int", nullable: false),
                    department_code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    department_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    sync_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sync_status = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.ForeignKey(
                        name: "fk_departments_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_departments_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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
                columns: new[] { "created_at", "department_id", "password" },
                values: new object[] { new DateTime(2023, 12, 20, 14, 19, 11, 766, DateTimeKind.Local).AddTicks(1403), null, "$2a$11$HuVVKjACc3U0kqqwP1uVVupn3F3HuT46ougsa5iTw5fkmMRvZOm5a" });

            migrationBuilder.CreateIndex(
                name: "ix_users_department_id",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_added_by",
                table: "departments",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_departments_modified_by",
                table: "departments",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_users_departments_department_id",
                table: "users",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_departments_department_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropIndex(
                name: "ix_users_department_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "department_id",
                table: "users");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 20, 9, 14, 33, 486, DateTimeKind.Local).AddTicks(205));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 20, 9, 14, 33, 218, DateTimeKind.Local).AddTicks(281), "$2a$11$.dmG8WMZlvtmtjEJV4b5z.J96Y9L4cnZjiIu1K7ADaEF2KGkqaJOS" });
        }
    }
}
