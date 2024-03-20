using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeReceiverInApproverTicketing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_level",
                table: "approver_ticketings");

            migrationBuilder.DropColumn(
                name: "receiver_id",
                table: "approver_ticketings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "current_level",
                table: "approver_ticketings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "receiver_id",
                table: "approver_ticketings",
                type: "int",
                nullable: true);
        }
    }
}
