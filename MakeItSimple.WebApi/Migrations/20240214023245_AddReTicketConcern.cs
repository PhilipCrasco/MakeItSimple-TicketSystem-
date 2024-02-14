using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddReTicketConcern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "re_ticket_concerns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    modified_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: false),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: false),
                    channel_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    concern_details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    sub_category_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_re_ticket = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    re_ticket_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    re_ticket_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    re_ticket_remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_reject_re_ticket = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    reject_transfer_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    reject_re_ticket_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    reject_remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ticket_approver = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    request_generator_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_re_ticket_concerns", x => x.id);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_request_generators_request_generator_id",
                        column: x => x.request_generator_id,
                        principalTable: "request_generators",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_re_ticket_by_user_id",
                        column: x => x.re_ticket_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_reject_re_ticket_by_user_id",
                        column: x => x.reject_re_ticket_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_added_by",
                table: "re_ticket_concerns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_category_id",
                table: "re_ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_channel_id",
                table: "re_ticket_concerns",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_department_id",
                table: "re_ticket_concerns",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_modified_by",
                table: "re_ticket_concerns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_re_ticket_by",
                table: "re_ticket_concerns",
                column: "re_ticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_reject_re_ticket_by",
                table: "re_ticket_concerns",
                column: "reject_re_ticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_request_generator_id",
                table: "re_ticket_concerns",
                column: "request_generator_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_sub_category_id",
                table: "re_ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_sub_unit_id",
                table: "re_ticket_concerns",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_ticket_concern_id",
                table: "re_ticket_concerns",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_user_id",
                table: "re_ticket_concerns",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "re_ticket_concerns");
        }
    }
}
