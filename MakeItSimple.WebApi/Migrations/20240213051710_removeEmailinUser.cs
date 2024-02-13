using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeEmailinUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "business_unit_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "email",
                table: "users");


            migrationBuilder.CreateTable(
                name: "business_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    modified_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    sync_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sync_status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    business_code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    business_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    company_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_business_units_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_business_units_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_business_units_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_added_by",
                table: "business_units",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_company_id",
                table: "business_units",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_modified_by",
                table: "business_units",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users",
                column: "business_unit_id",
                principalTable: "business_units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_business_units_business_unit_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "business_units");

            migrationBuilder.RenameColumn(
                name: "business_unit_id",
                table: "users",
                newName: "account_title_id");

            migrationBuilder.RenameIndex(
                name: "ix_users_business_unit_id",
                table: "users",
                newName: "ix_users_account_title_id");

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "email",
                value: "admin@gmail.com");

            migrationBuilder.AddForeignKey(
                name: "fk_users_account_titles_account_title_id",
                table: "users",
                column: "account_title_id",
                principalTable: "account_titles",
                principalColumn: "id");
        }
    }
}
