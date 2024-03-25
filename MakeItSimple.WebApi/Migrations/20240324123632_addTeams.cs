 using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "channels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "approvers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "team_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    added_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    modified_by = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    team_name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_teams_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_teams_users_modified_by_user_id",
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
                column: "team_id",
                value: null);

            migrationBuilder.CreateIndex(
                name: "ix_users_team_id",
                table: "users",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_team_id",
                table: "channels",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_team_id",
                table: "approvers",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_team_id",
                table: "approver_ticketings",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "ix_teams_added_by",
                table: "teams",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_teams_modified_by",
                table: "teams",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_teams_sub_unit_id",
                table: "teams",
                column: "sub_unit_id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_teams_team_id",
                table: "approver_ticketings",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_teams_team_id",
                table: "approvers",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_teams_team_id",
                table: "channels",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_teams_team_id",
                table: "users",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_teams_team_id",
                table: "approver_ticketings");

            migrationBuilder.DropForeignKey(
                name: "fk_approvers_teams_team_id",
                table: "approvers");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_teams_team_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_users_teams_team_id",
                table: "users");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropIndex(
                name: "ix_users_team_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_channels_team_id",
                table: "channels");

            migrationBuilder.DropIndex(
                name: "ix_approvers_team_id",
                table: "approvers");

            migrationBuilder.DropIndex(
                name: "ix_approver_ticketings_team_id",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "approvers");

            migrationBuilder.DropColumn(
                name: "team_id",
                table: "approver_ticketings");
        }
    }
}
