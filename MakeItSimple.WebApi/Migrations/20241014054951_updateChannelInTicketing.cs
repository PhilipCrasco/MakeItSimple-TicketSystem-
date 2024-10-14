using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateChannelInTicketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_channels_channel_id",
                table: "ticket_concerns");

            migrationBuilder.DropIndex(
                name: "ix_ticket_concerns_channel_id",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "request_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_channel_id",
                table: "request_concerns",
                column: "channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_channels_channel_id",
                table: "request_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_channels_channel_id",
                table: "request_concerns");

            migrationBuilder.DropIndex(
                name: "ix_request_concerns_channel_id",
                table: "request_concerns");

            migrationBuilder.DropColumn(
                name: "channel_id",
                table: "request_concerns");

            migrationBuilder.AddColumn<int>(
                name: "channel_id",
                table: "ticket_concerns",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_channel_id",
                table: "ticket_concerns",
                column: "channel_id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_channels_channel_id",
                table: "ticket_concerns",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");
        }
    }
}
