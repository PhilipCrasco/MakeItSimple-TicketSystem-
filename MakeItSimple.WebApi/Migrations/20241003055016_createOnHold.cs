using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class createOnHold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "on_hold",
                table: "ticket_concerns",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "on_hold_at",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "on_hold_reason",
                table: "ticket_concerns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "resume_at",
                table: "ticket_concerns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "expected_date",
                table: "request_concerns",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "on_hold",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "on_hold_at",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "on_hold_reason",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "resume_at",
                table: "ticket_concerns");

            migrationBuilder.DropColumn(
                name: "expected_date",
                table: "request_concerns");
        }
    }
}
