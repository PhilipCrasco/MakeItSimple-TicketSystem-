using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "transfer_ticket_concern",
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
                    sub_unit_id = table.Column<int>(type: "int", nullable: false),
                    channel_id = table.Column<int>(type: "int", nullable: false),
                    concern_details = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    category_id = table.Column<int>(type: "int", nullable: false),
                    sub_category_id = table.Column<int>(type: "int", nullable: false),
                    start_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    is_transfer = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    transfer_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    transfer_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    transfer_remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfer_ticket_concern", x => x.id);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_users_transfer_by_user_id",
                        column: x => x.transfer_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concern_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_added_by",
                table: "transfer_ticket_concern",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_category_id",
                table: "transfer_ticket_concern",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_channel_id",
                table: "transfer_ticket_concern",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_modified_by",
                table: "transfer_ticket_concern",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_sub_category_id",
                table: "transfer_ticket_concern",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_sub_unit_id",
                table: "transfer_ticket_concern",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_ticket_concern_id",
                table: "transfer_ticket_concern",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_transfer_by",
                table: "transfer_ticket_concern",
                column: "transfer_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concern_user_id",
                table: "transfer_ticket_concern",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "transfer_ticket_concern");
        }
    }
}
