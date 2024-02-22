using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addUnitAndEditLocationSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "location_id",
                table: "sub_units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "sub_units",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "unit_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    modified_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    unit_no = table.Column<int>(type: "int", nullable: false),
                    unit_code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    unit_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    sync_status = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_units_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_units_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_units_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "unit_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_users_unit_id",
                table: "users",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_unit_id",
                table: "transfer_ticket_concerns",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_unit_id",
                table: "ticket_concerns",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_location_id",
                table: "sub_units",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_unit_id",
                table: "sub_units",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_unit_id",
                table: "re_ticket_concerns",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_units_added_by",
                table: "units",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_units_department_id",
                table: "units",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_units_modified_by",
                table: "units",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_units_unit_id",
                table: "re_ticket_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sub_units_locations_location_id",
                table: "sub_units",
                column: "location_id",
                principalTable: "locations",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sub_units_units_unit_id",
                table: "sub_units",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_units_unit_id",
                table: "ticket_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_units_unit_id",
                table: "transfer_ticket_concerns",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_units_unit_id",
                table: "users",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_units_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_sub_units_locations_location_id",
                table: "sub_units");

            migrationBuilder.DropForeignKey(
                name: "fk_sub_units_units_unit_id",
                table: "sub_units");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_units_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_units_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_users_units_unit_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropIndex(
                name: "ix_users_unit_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_transfer_ticket_concerns_unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_sub_units_location_id",
                table: "sub_units");

            migrationBuilder.DropIndex(
                name: "ix_sub_units_unit_id",
                table: "sub_units");

            migrationBuilder.DropIndex(
                name: "ix_re_ticket_concerns_unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "location_id",
                table: "sub_units");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "sub_units");

            migrationBuilder.DropColumn(
                name: "unit_id",
                table: "re_ticket_concerns");

            migrationBuilder.AddColumn<int>(
                name: "sub_unit_id",
                table: "locations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_locations_sub_unit_id",
                table: "locations",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_locations_sub_units_sub_unit_id",
                table: "locations",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");
        }
    }
}
