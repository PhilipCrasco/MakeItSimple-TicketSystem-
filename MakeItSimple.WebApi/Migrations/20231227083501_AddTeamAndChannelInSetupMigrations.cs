using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamAndChannelInSetupMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_added_by_user_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels");

            migrationBuilder.DropIndex(
                name: "ix_channels_user_id1",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "channels");

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "user_id",
                table: "channels",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 27, 16, 35, 1, 311, DateTimeKind.Local).AddTicks(9552));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "channel_id", "created_at", "password" },
                values: new object[] { null, new DateTime(2023, 12, 27, 16, 35, 1, 311, DateTimeKind.Local).AddTicks(4199), "$2a$12$ihvpKbpvdRfZLXz.tZKFEulxnTg1tiS11T/MbpufId3rzXoCMW2OK" });

            migrationBuilder.CreateIndex(
                name: "ix_users_channel_id",
                table: "users",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_user_id",
                table: "channels",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_user_id",
                table: "channels",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_channels_channel_id",
                table: "users",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_users_channels_channel_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_channel_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_channels_user_id",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "users");

            migrationBuilder.AlterColumn<int>(
                name: "user_id",
                table: "channels",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "user_id1",
                table: "channels",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

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
                columns: new[] { "created_at", "password" },
                values: new object[] { new DateTime(2023, 12, 27, 11, 44, 58, 432, DateTimeKind.Local).AddTicks(8998), "$2a$11$.v3LliNkzKhlUYNPLC0Ybu3him.5AqclKf7ycm1ak336eNio7z4KG" });

            migrationBuilder.CreateIndex(
                name: "ix_channels_user_id1",
                table: "channels",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_added_by_user_id",
                table: "channels",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels",
                column: "user_id1",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
