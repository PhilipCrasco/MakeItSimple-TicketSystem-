using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class nullableChannelId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_channels_channel_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_channels_channel_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_channels_channel_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "closing_tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_channels_channel_id",
                table: "closing_tickets",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_channels_channel_id",
                table: "re_ticket_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_channels_channel_id",
                table: "transfer_ticket_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_closing_tickets_channels_channel_id",
                table: "closing_tickets");

            migrationBuilder.DropForeignKey(
                name: "fk_re_ticket_concerns_channels_channel_id",
                table: "re_ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_transfer_ticket_concerns_channels_channel_id",
                table: "transfer_ticket_concerns");

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "transfer_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "re_ticket_concerns",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "closing_tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_channels_channel_id",
                table: "closing_tickets",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_channels_channel_id",
                table: "re_ticket_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_channels_channel_id",
                table: "transfer_ticket_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
