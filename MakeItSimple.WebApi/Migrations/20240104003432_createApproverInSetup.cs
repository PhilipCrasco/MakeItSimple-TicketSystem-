using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createApproverInSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approvers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    modified_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    channel_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    approver_level = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approvers", x => x.id);
                    table.ForeignKey(
                        name: "fk_approvers_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_approvers_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_approvers_users_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_approvers_users_user_id1",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2024, 1, 4, 8, 34, 31, 549, DateTimeKind.Local).AddTicks(8973));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "created_at",
                value: new DateTime(2024, 1, 4, 8, 34, 31, 549, DateTimeKind.Local).AddTicks(3253));

            migrationBuilder.CreateIndex(
                name: "ix_approvers_added_by",
                table: "approvers",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_channel_id",
                table: "approvers",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_modified_by",
                table: "approvers",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_user_id",
                table: "approvers",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approvers");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2024, 1, 1, 20, 14, 19, 653, DateTimeKind.Local).AddTicks(8924));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "created_at",
                value: new DateTime(2024, 1, 1, 20, 14, 19, 653, DateTimeKind.Local).AddTicks(3423));
        }
    }
}
