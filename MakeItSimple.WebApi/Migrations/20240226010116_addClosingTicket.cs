using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addClosingTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "closing_tickets",
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
                    unit_id = table.Column<int>(type: "int", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: false),
                    channel_id = table.Column<int>(type: "int", nullable: false),
                    concern_details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    sub_category_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    is_closing = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    closing_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    closed_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    closing_remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_reject_closed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    reject_closed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    reject_closed_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    reject_remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ticket_approver = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    request_generator_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_closing_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_closing_tickets_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_closing_tickets_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_closing_tickets_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_closing_tickets_request_generators_request_generator_id",
                        column: x => x.request_generator_id,
                        principalTable: "request_generators",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_closing_tickets_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_closing_tickets_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_closing_tickets_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_closing_tickets_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_closing_tickets_users_closed_by_user_id",
                        column: x => x.closed_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_closing_tickets_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_closing_tickets_users_reject_closed_by_user_id",
                        column: x => x.reject_closed_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_closing_tickets_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_closing_tickets_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_added_by",
                table: "closing_tickets",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_category_id",
                table: "closing_tickets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_channel_id",
                table: "closing_tickets",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_closed_by",
                table: "closing_tickets",
                column: "closed_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_department_id",
                table: "closing_tickets",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_modified_by",
                table: "closing_tickets",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_reject_closed_by",
                table: "closing_tickets",
                column: "reject_closed_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_request_generator_id",
                table: "closing_tickets",
                column: "request_generator_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_sub_unit_id",
                table: "closing_tickets",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_ticket_concern_id",
                table: "closing_tickets",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_unit_id",
                table: "closing_tickets",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_user_id",
                table: "closing_tickets",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "closing_tickets");
        }
    }
}
