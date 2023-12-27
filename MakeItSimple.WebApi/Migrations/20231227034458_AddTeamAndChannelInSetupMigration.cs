using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndChannelInSetupMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_channel_team_team_id",
                table: "channel");

            migrationBuilder.DropForeignKey(
                name: "fk_channel_users_added_by_user_id",
                table: "channel");

            migrationBuilder.DropForeignKey(
                name: "fk_channel_users_modified_by_user_id",
                table: "channel");

            migrationBuilder.DropForeignKey(
                name: "fk_channel_users_user_id1",
                table: "channel");

            migrationBuilder.DropForeignKey(
                name: "fk_team_users_added_by_user_id",
                table: "team");

            migrationBuilder.DropForeignKey(
                name: "fk_team_users_modified_by_user_id",
                table: "team");

            migrationBuilder.DropPrimaryKey(
                name: "pk_team",
                table: "team");

            migrationBuilder.DropPrimaryKey(
                name: "pk_channel",
                table: "channel");

            migrationBuilder.RenameTable(
                name: "team",
                newName: "teams");

            migrationBuilder.RenameTable(
                name: "channel",
                newName: "channels");

            migrationBuilder.RenameIndex(
                name: "ix_team_modified_by",
                table: "teams",
                newName: "ix_teams_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_team_added_by",
                table: "teams",
                newName: "ix_teams_added_by");

            migrationBuilder.RenameIndex(
                name: "ix_channel_user_id1",
                table: "channels",
                newName: "ix_channels_user_id1");

            migrationBuilder.RenameIndex(
                name: "ix_channel_team_id",
                table: "channels",
                newName: "ix_channels_team_id");

            migrationBuilder.RenameIndex(
                name: "ix_channel_modified_by",
                table: "channels",
                newName: "ix_channels_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_channel_added_by",
                table: "channels",
                newName: "ix_channels_added_by");

            migrationBuilder.AddPrimaryKey(
                name: "pk_teams",
                table: "teams",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_channels",
                table: "channels",
                column: "id");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 27, 11, 44, 58, 433, DateTimeKind.Local).AddTicks(3947));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "created_at",
                value: new DateTime(2023, 12, 27, 11, 44, 58, 432, DateTimeKind.Local).AddTicks(8998));

            migrationBuilder.AddForeignKey(
                name: "fk_channels_teams_team_id",
                table: "channels",
                column: "team_id",
                principalTable: "teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_added_by_user_id",
                table: "channels",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_modified_by_user_id",
                table: "channels",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels",
                column: "user_id1",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_teams_users_added_by_user_id",
                table: "teams",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_teams_users_modified_by_user_id",
                table: "teams",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_channels_teams_team_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_added_by_user_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_modified_by_user_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_users_added_by_user_id",
                table: "teams");

            migrationBuilder.DropForeignKey(
                name: "fk_teams_users_modified_by_user_id",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "pk_teams",
                table: "teams");

            migrationBuilder.DropPrimaryKey(
                name: "pk_channels",
                table: "channels");

            migrationBuilder.RenameTable(
                name: "teams",
                newName: "team");

            migrationBuilder.RenameTable(
                name: "channels",
                newName: "channel");

            migrationBuilder.RenameIndex(
                name: "ix_teams_modified_by",
                table: "team",
                newName: "ix_team_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_teams_added_by",
                table: "team",
                newName: "ix_team_added_by");

            migrationBuilder.RenameIndex(
                name: "ix_channels_user_id1",
                table: "channel",
                newName: "ix_channel_user_id1");

            migrationBuilder.RenameIndex(
                name: "ix_channels_team_id",
                table: "channel",
                newName: "ix_channel_team_id");

            migrationBuilder.RenameIndex(
                name: "ix_channels_modified_by",
                table: "channel",
                newName: "ix_channel_modified_by");

            migrationBuilder.RenameIndex(
                name: "ix_channels_added_by",
                table: "channel",
                newName: "ix_channel_added_by");

            migrationBuilder.AddPrimaryKey(
                name: "pk_team",
                table: "team",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_channel",
                table: "channel",
                column: "id");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 27, 11, 37, 2, 178, DateTimeKind.Local).AddTicks(4247));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "created_at",
                value: new DateTime(2023, 12, 27, 11, 37, 1, 902, DateTimeKind.Local).AddTicks(6813));

            migrationBuilder.AddForeignKey(
                name: "fk_channel_team_team_id",
                table: "channel",
                column: "team_id",
                principalTable: "team",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_channel_users_added_by_user_id",
                table: "channel",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channel_users_modified_by_user_id",
                table: "channel",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channel_users_user_id1",
                table: "channel",
                column: "user_id1",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_team_users_added_by_user_id",
                table: "team",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_team_users_modified_by_user_id",
                table: "team",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
