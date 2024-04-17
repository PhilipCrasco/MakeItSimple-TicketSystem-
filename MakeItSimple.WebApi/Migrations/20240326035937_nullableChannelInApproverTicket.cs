using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class nullableChannelInApproverTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings");

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings");

            migrationBuilder.AlterColumn<int>(
                name: "channel_id",
                table: "approver_ticketings",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
