using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddSubCategoryInUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_channels_channel_id",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_channel_id",
                table: "users");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "users");

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 28, 14, 4, 27, 824, DateTimeKind.Local).AddTicks(2891));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                column: "created_at",
                value: new DateTime(2023, 12, 28, 14, 4, 27, 823, DateTimeKind.Local).AddTicks(8270));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "created_at",
                value: new DateTime(2023, 12, 28, 13, 46, 43, 563, DateTimeKind.Local).AddTicks(8873));

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "id",
                keyValue: new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"),
                columns: new[] { "channel_id", "created_at" },
                values: new object[] { null, new DateTime(2023, 12, 28, 13, 46, 43, 563, DateTimeKind.Local).AddTicks(3665) });

            migrationBuilder.CreateIndex(
                name: "ix_users_channel_id",
                table: "users",
                column: "channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_channels_channel_id",
                table: "users",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }
    }
}
