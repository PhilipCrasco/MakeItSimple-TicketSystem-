using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class removeprojects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approvers_users_modified_by_user_id",
                table: "approvers");

            migrationBuilder.DropForeignKey(
                name: "fk_approvers_users_user_id1",
                table: "approvers");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_projects_project_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_modified_by_user_id",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_receivers_users_modified_by_user_id",
                table: "receivers");

            migrationBuilder.DropForeignKey(
                name: "fk_receivers_users_user_id1",
                table: "receivers");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_modified_by_user_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_reject_by_user_id",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_approved_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_closed_approve_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_modified_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_re_date_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_requestor_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_reticket_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_transfer_by_user_id",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropIndex(
                name: "ix_channels_project_id",
                table: "channels");

            migrationBuilder.DropColumn(
                name: "project_id",
                table: "channels");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_users_user_id1",
                table: "approvers",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_users_user_id11",
                table: "approvers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_user_id11",
                table: "channels",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_receivers_users_user_id1",
                table: "receivers",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_receivers_users_user_id11",
                table: "receivers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id11",
                table: "request_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id2",
                table: "request_concerns",
                column: "reject_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns",
                column: "approved_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id11",
                table: "ticket_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id2",
                table: "ticket_concerns",
                column: "closed_approve_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id3",
                table: "ticket_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns",
                column: "re_date_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id6",
                table: "ticket_concerns",
                column: "reticket_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id7",
                table: "ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_approvers_users_user_id1",
                table: "approvers");

            migrationBuilder.DropForeignKey(
                name: "fk_approvers_users_user_id11",
                table: "approvers");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id1",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_channels_users_user_id11",
                table: "channels");

            migrationBuilder.DropForeignKey(
                name: "fk_receivers_users_user_id1",
                table: "receivers");

            migrationBuilder.DropForeignKey(
                name: "fk_receivers_users_user_id11",
                table: "receivers");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id11",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_request_concerns_users_user_id2",
                table: "request_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id11",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id2",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id3",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id4",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id5",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id6",
                table: "ticket_concerns");

            migrationBuilder.DropForeignKey(
                name: "fk_ticket_concerns_users_user_id7",
                table: "ticket_concerns");

            migrationBuilder.AddColumn<int>(
                name: "project_id",
                table: "channels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    project_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                    table.ForeignKey(
                        name: "fk_projects_users_added_by_user_id",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_projects_users_modified_by_user_id",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_channels_project_id",
                table: "channels",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_added_by",
                table: "projects",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_projects_modified_by",
                table: "projects",
                column: "modified_by");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_users_modified_by_user_id",
                table: "approvers",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_users_user_id1",
                table: "approvers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_projects_project_id",
                table: "channels",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

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
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_receivers_users_modified_by_user_id",
                table: "receivers",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_receivers_users_user_id1",
                table: "receivers",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_modified_by_user_id",
                table: "request_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_reject_by_user_id",
                table: "request_concerns",
                column: "reject_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_request_concerns_users_user_id1",
                table: "request_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_approved_by_user_id",
                table: "ticket_concerns",
                column: "approved_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_closed_approve_by_user_id",
                table: "ticket_concerns",
                column: "closed_approve_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_modified_by_user_id",
                table: "ticket_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_re_date_by_user_id",
                table: "ticket_concerns",
                column: "re_date_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_requestor_by_user_id",
                table: "ticket_concerns",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_reticket_by_user_id",
                table: "ticket_concerns",
                column: "reticket_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_transfer_by_user_id",
                table: "ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");
        }
    }
}
