using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class adddatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "request_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ticket_transactions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_transactions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "account_titles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    account_no = table.Column<int>(type: "int", nullable: false),
                    account_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    account_titles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_account_titles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "approver_ticketings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_approve = table.Column<bool>(type: "bit", nullable: true),
                    approver_level = table.Column<int>(type: "int", nullable: true),
                    issue_handler = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approver_ticketings", x => x.id);
                    table.ForeignKey(
                        name: "fk_approver_ticketings_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_approver_ticketings_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "approvers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    approver_level = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approvers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "business_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_no = table.Column<int>(type: "int", nullable: true),
                    business_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_units", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    category_description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "channel_users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channel_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "channels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    channel_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    project_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_channels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "closing_tickets",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: false),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    concern_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    sub_category_id = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_closing = table.Column<bool>(type: "bit", nullable: true),
                    closing_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    closed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    closing_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_reject_closed = table.Column<bool>(type: "bit", nullable: false),
                    reject_closed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reject_closed_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_closing_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_closing_tickets_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_closing_tickets_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_closing_tickets_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_closing_tickets_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    company_no = table.Column<int>(type: "int", nullable: false),
                    company_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    company_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "departments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    department_no = table.Column<int>(type: "int", nullable: false),
                    department_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    business_unit_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_departments", x => x.id);
                    table.ForeignKey(
                        name: "fk_departments_business_units_business_unit_id",
                        column: x => x.business_unit_id,
                        principalTable: "business_units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "locations",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    location_no = table.Column<int>(type: "int", nullable: false),
                    location_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    location_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    manual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_locations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projects",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    project_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projects", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "re_ticket_concerns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: false),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    concern_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    sub_category_id = table.Column<int>(type: "int", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_re_ticket = table.Column<bool>(type: "bit", nullable: true),
                    re_ticket_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    re_ticket_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    re_ticket_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_reject_re_ticket = table.Column<bool>(type: "bit", nullable: false),
                    reject_re_ticket_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reject_re_ticket_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_re_ticket_concerns", x => x.id);
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_re_ticket_concerns_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "receivers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    business_unit_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_receivers", x => x.id);
                    table.ForeignKey(
                        name: "fk_receivers_business_units_business_unit_id",
                        column: x => x.business_unit_id,
                        principalTable: "business_units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "request_concerns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    concern_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_reject = table.Column<bool>(type: "bit", nullable: false),
                    reject_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_done = table.Column<bool>(type: "bit", nullable: true),
                    concern = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_request_concerns", x => x.id);
                    table.ForeignKey(
                        name: "fk_request_concerns_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "sub_categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_category_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sub_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_sub_categories_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sub_units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_unit_no = table.Column<int>(type: "int", nullable: true),
                    sub_unit_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sub_unit_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    unit_id = table.Column<int>(type: "int", nullable: true),
                    manual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sub_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_sub_units_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    team_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_teams", x => x.id);
                    table.ForeignKey(
                        name: "fk_teams_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ticket_attachments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_size = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_attachments_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_attachments_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ticket_comment_views",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_clicked = table.Column<bool>(type: "bit", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_comment_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_comment_views", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_comment_views_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ticket_comments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    file_size = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    is_clicked = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_comments_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ticket_concerns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    concern_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    sub_category_id = table.Column<int>(type: "int", nullable: true),
                    is_transfer = table.Column<bool>(type: "bit", nullable: true),
                    transfer_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    transfer_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_approve = table.Column<bool>(type: "bit", nullable: true),
                    approved_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    approved_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_closed_approve = table.Column<bool>(type: "bit", nullable: true),
                    closed_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    closed_approve_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_closed_reject = table.Column<bool>(type: "bit", nullable: false),
                    is_reject = table.Column<bool>(type: "bit", nullable: false),
                    is_re_ticket = table.Column<bool>(type: "bit", nullable: true),
                    reticket_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reticket_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    target_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    is_done = table.Column<bool>(type: "bit", nullable: true),
                    concern_status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticket_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    requestor_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    request_concern_id = table.Column<int>(type: "int", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_assigned = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_concerns", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_concerns_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_concerns_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_concerns_request_concerns_request_concern_id",
                        column: x => x.request_concern_id,
                        principalTable: "request_concerns",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_concerns_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_concerns_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "ticket_histories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true),
                    requestor_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    approver_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    request = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transaction_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ticket_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_ticket_histories_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_ticket_histories_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "transfer_ticket_concerns",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ticket_concern_id = table.Column<int>(type: "int", nullable: false),
                    channel_id = table.Column<int>(type: "int", nullable: true),
                    concern_details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    sub_category_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    is_transfer = table.Column<bool>(type: "bit", nullable: true),
                    transfer_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    transfer_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    transfer_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_reject_transfer = table.Column<bool>(type: "bit", nullable: false),
                    reject_transfer_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    reject_transfer_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    reject_remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ticket_approver = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    receiver_id = table.Column<int>(type: "int", nullable: true),
                    request_transaction_id = table.Column<int>(type: "int", nullable: true),
                    ticket_transaction_id = table.Column<int>(type: "int", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transfer_ticket_concerns", x => x.id);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concerns_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concerns_channels_channel_id",
                        column: x => x.channel_id,
                        principalTable: "channels",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concerns_request_transactions_request_transaction_id",
                        column: x => x.request_transaction_id,
                        principalTable: "request_transactions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concerns_sub_categories_sub_category_id",
                        column: x => x.sub_category_id,
                        principalTable: "sub_categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concerns_ticket_concerns_ticket_concern_id",
                        column: x => x.ticket_concern_id,
                        principalTable: "ticket_concerns",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_transfer_ticket_concerns_ticket_transactions_ticket_transaction_id",
                        column: x => x.ticket_transaction_id,
                        principalTable: "ticket_transactions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "units",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    unit_no = table.Column<int>(type: "int", nullable: false),
                    unit_code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    unit_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    sync_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    sync_status = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_units", x => x.id);
                    table.ForeignKey(
                        name: "fk_units_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_role_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    permissions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    emp_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_password_change = table.Column<bool>(type: "bit", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    added_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modified_by = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_role_id = table.Column<int>(type: "int", nullable: false),
                    company_id = table.Column<int>(type: "int", nullable: true),
                    business_unit_id = table.Column<int>(type: "int", nullable: true),
                    department_id = table.Column<int>(type: "int", nullable: true),
                    unit_id = table.Column<int>(type: "int", nullable: true),
                    sub_unit_id = table.Column<int>(type: "int", nullable: true),
                    location_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_business_units_business_unit_id",
                        column: x => x.business_unit_id,
                        principalTable: "business_units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_departments_department_id",
                        column: x => x.department_id,
                        principalTable: "departments",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_sub_units_sub_unit_id",
                        column: x => x.sub_unit_id,
                        principalTable: "sub_units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_units_unit_id",
                        column: x => x.unit_id,
                        principalTable: "units",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_users_user_roles_user_role_id",
                        column: x => x.user_role_id,
                        principalTable: "user_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_users_added_by",
                        column: x => x.added_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_users_users_modified_by",
                        column: x => x.modified_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "user_roles",
                columns: new[] { "id", "added_by", "created_at", "is_active", "modified_by", "permissions", "updated_at", "user_role_name" },
                values: new object[] { 1, null, new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), true, null, "[\"Overview\",\"User Management\",\"User Role\",\"User Account\",\"Channel\",\"Filing\",\"Generate\",\"Masterlist\",\"Company\",\"Business Unit\",\"Unit\",\"Location\",\"Sub Unit\",\"Department\",\"Category\",\"Sub Category\",\"Channel Setup\",\"Approver\",\"Receiver Concerns\",\"Receiver\"]", null, "Admin" });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "id", "added_by", "business_unit_id", "company_id", "created_at", "department_id", "emp_id", "fullname", "is_active", "is_password_change", "location_id", "modified_by", "password", "sub_unit_id", "unit_id", "updated_at", "user_role_id", "username" },
                values: new object[] { new Guid("bca9f29a-ccfb-4cd5-aa51-f3f61ea635d2"), null, null, null, new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, "Admin", true, true, null, null, "$2a$12$ihvpKbpvdRfZLXz.tZKFEulxnTg1tiS11T/MbpufId3rzXoCMW2OK", null, null, null, 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "ix_account_titles_added_by",
                table: "account_titles",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_account_titles_modified_by",
                table: "account_titles",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_added_by",
                table: "approver_ticketings",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_channel_id",
                table: "approver_ticketings",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_request_transaction_id",
                table: "approver_ticketings",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_sub_unit_id",
                table: "approver_ticketings",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_ticket_transaction_id",
                table: "approver_ticketings",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_approver_ticketings_user_id",
                table: "approver_ticketings",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_added_by",
                table: "approvers",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_channel_id",
                table: "approvers",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_modified_by",
                table: "approvers",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_sub_unit_id",
                table: "approvers",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_approvers_user_id",
                table: "approvers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_added_by",
                table: "business_units",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_company_id",
                table: "business_units",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_business_units_modified_by",
                table: "business_units",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_categories_added_by",
                table: "categories",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_categories_modified_by",
                table: "categories",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_channel_users_channel_id",
                table: "channel_users",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_channel_users_user_id",
                table: "channel_users",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_added_by",
                table: "channels",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_channels_department_id",
                table: "channels",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_modified_by",
                table: "channels",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_channels_project_id",
                table: "channels",
                column: "project_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_sub_unit_id",
                table: "channels",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_channels_user_id",
                table: "channels",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_added_by",
                table: "closing_tickets",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_category_id",
                table: "closing_tickets",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_channel_id",
                table: "closing_tickets",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_closed_by",
                table: "closing_tickets",
                column: "closed_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_modified_by",
                table: "closing_tickets",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_reject_closed_by",
                table: "closing_tickets",
                column: "reject_closed_by");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_request_transaction_id",
                table: "closing_tickets",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_ticket_concern_id",
                table: "closing_tickets",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_ticket_transaction_id",
                table: "closing_tickets",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_closing_tickets_user_id",
                table: "closing_tickets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_companies_added_by",
                table: "companies",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_companies_modified_by",
                table: "companies",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_departments_added_by",
                table: "departments",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_departments_business_unit_id",
                table: "departments",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_departments_modified_by",
                table: "departments",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_locations_added_by",
                table: "locations",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_locations_modified_by",
                table: "locations",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_locations_sub_unit_id",
                table: "locations",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_projects_added_by",
                table: "projects",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_projects_modified_by",
                table: "projects",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_added_by",
                table: "re_ticket_concerns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_category_id",
                table: "re_ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_channel_id",
                table: "re_ticket_concerns",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_modified_by",
                table: "re_ticket_concerns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_re_ticket_by",
                table: "re_ticket_concerns",
                column: "re_ticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_reject_re_ticket_by",
                table: "re_ticket_concerns",
                column: "reject_re_ticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_request_transaction_id",
                table: "re_ticket_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_sub_category_id",
                table: "re_ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_ticket_concern_id",
                table: "re_ticket_concerns",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_ticket_transaction_id",
                table: "re_ticket_concerns",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_re_ticket_concerns_user_id",
                table: "re_ticket_concerns",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_receivers_added_by",
                table: "receivers",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_receivers_business_unit_id",
                table: "receivers",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_receivers_modified_by",
                table: "receivers",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_receivers_user_id",
                table: "receivers",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_added_by",
                table: "request_concerns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_modified_by",
                table: "request_concerns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_reject_by",
                table: "request_concerns",
                column: "reject_by");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_request_transaction_id",
                table: "request_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_request_concerns_user_id",
                table: "request_concerns",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_sub_categories_added_by",
                table: "sub_categories",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_sub_categories_category_id",
                table: "sub_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_sub_categories_modified_by",
                table: "sub_categories",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_added_by",
                table: "sub_units",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_department_id",
                table: "sub_units",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_modified_by",
                table: "sub_units",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_sub_units_unit_id",
                table: "sub_units",
                column: "unit_id");

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

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_added_by",
                table: "ticket_attachments",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_modified_by",
                table: "ticket_attachments",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_request_transaction_id",
                table: "ticket_attachments",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_attachments_ticket_transaction_id",
                table: "ticket_attachments",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_added_by",
                table: "ticket_comment_views",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_request_transaction_id",
                table: "ticket_comment_views",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_ticket_comment_id",
                table: "ticket_comment_views",
                column: "ticket_comment_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comment_views_user_id",
                table: "ticket_comment_views",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comments_added_by",
                table: "ticket_comments",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comments_modified_by",
                table: "ticket_comments",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_comments_request_transaction_id",
                table: "ticket_comments",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_added_by",
                table: "ticket_concerns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_approved_by",
                table: "ticket_concerns",
                column: "approved_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_category_id",
                table: "ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_channel_id",
                table: "ticket_concerns",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_closed_approve_by",
                table: "ticket_concerns",
                column: "closed_approve_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_modified_by",
                table: "ticket_concerns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_request_concern_id",
                table: "ticket_concerns",
                column: "request_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_request_transaction_id",
                table: "ticket_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_requestor_by",
                table: "ticket_concerns",
                column: "requestor_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_reticket_by",
                table: "ticket_concerns",
                column: "reticket_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_sub_category_id",
                table: "ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_transfer_by",
                table: "ticket_concerns",
                column: "transfer_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_concerns_user_id",
                table: "ticket_concerns",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_approver_by",
                table: "ticket_histories",
                column: "approver_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_request_transaction_id",
                table: "ticket_histories",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_requestor_by",
                table: "ticket_histories",
                column: "requestor_by");

            migrationBuilder.CreateIndex(
                name: "ix_ticket_histories_ticket_transaction_id",
                table: "ticket_histories",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_added_by",
                table: "transfer_ticket_concerns",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_category_id",
                table: "transfer_ticket_concerns",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_channel_id",
                table: "transfer_ticket_concerns",
                column: "channel_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_modified_by",
                table: "transfer_ticket_concerns",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_reject_transfer_by",
                table: "transfer_ticket_concerns",
                column: "reject_transfer_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_request_transaction_id",
                table: "transfer_ticket_concerns",
                column: "request_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_sub_category_id",
                table: "transfer_ticket_concerns",
                column: "sub_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_ticket_concern_id",
                table: "transfer_ticket_concerns",
                column: "ticket_concern_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_ticket_transaction_id",
                table: "transfer_ticket_concerns",
                column: "ticket_transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_transfer_by",
                table: "transfer_ticket_concerns",
                column: "transfer_by");

            migrationBuilder.CreateIndex(
                name: "ix_transfer_ticket_concerns_user_id",
                table: "transfer_ticket_concerns",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_units_added_by",
                table: "units",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_units_department_id",
                table: "units",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_units_modified_by",
                table: "units",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_added_by",
                table: "user_roles",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_modified_by",
                table: "user_roles",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_users_added_by",
                table: "users",
                column: "added_by");

            migrationBuilder.CreateIndex(
                name: "ix_users_business_unit_id",
                table: "users",
                column: "business_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_company_id",
                table: "users",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_department_id",
                table: "users",
                column: "department_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_location_id",
                table: "users",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_modified_by",
                table: "users",
                column: "modified_by");

            migrationBuilder.CreateIndex(
                name: "ix_users_sub_unit_id",
                table: "users",
                column: "sub_unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_unit_id",
                table: "users",
                column: "unit_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_user_role_id",
                table: "users",
                column: "user_role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_account_titles_users_added_by_user_id",
                table: "account_titles",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_account_titles_users_modified_by_user_id",
                table: "account_titles",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_channels_channel_id",
                table: "approver_ticketings",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_sub_units_sub_unit_id",
                table: "approver_ticketings",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_users_user_id",
                table: "approver_ticketings",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_approver_ticketings_users_user_id1",
                table: "approver_ticketings",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_channels_channel_id",
                table: "approvers",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_sub_units_sub_unit_id",
                table: "approvers",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_users_modified_by_user_id",
                table: "approvers",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_approvers_users_user_id",
                table: "approvers",
                column: "added_by",
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
                name: "fk_business_units_companies_company_id",
                table: "business_units",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_business_units_users_added_by_user_id",
                table: "business_units",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_business_units_users_modified_by_user_id",
                table: "business_units",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_users_added_by_user_id",
                table: "categories",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_categories_users_modified_by_user_id",
                table: "categories",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_channel_users_channels_channel_id",
                table: "channel_users",
                column: "channel_id",
                principalTable: "channels",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_channel_users_users_user_id",
                table: "channel_users",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_departments_department_id",
                table: "channels",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_projects_project_id",
                table: "channels",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_sub_units_sub_unit_id",
                table: "channels",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_channels_users_modified_by_user_id",
                table: "channels",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_closing_tickets_sub_categories_sub_category_id",
                table: "closing_tickets",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_ticket_concerns_ticket_concern_id",
                table: "closing_tickets",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_closed_by_user_id",
                table: "closing_tickets",
                column: "closed_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_modified_by_user_id",
                table: "closing_tickets",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_reject_closed_by_user_id",
                table: "closing_tickets",
                column: "reject_closed_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_user_id",
                table: "closing_tickets",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_closing_tickets_users_user_id1",
                table: "closing_tickets",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_companies_users_added_by_user_id",
                table: "companies",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_companies_users_modified_by_user_id",
                table: "companies",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_departments_users_added_by_user_id",
                table: "departments",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_departments_users_modified_by_user_id",
                table: "departments",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_locations_sub_units_sub_unit_id",
                table: "locations",
                column: "sub_unit_id",
                principalTable: "sub_units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_locations_users_added_by_user_id",
                table: "locations",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_locations_users_modified_by_user_id",
                table: "locations",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_users_added_by_user_id",
                table: "projects",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_projects_users_modified_by_user_id",
                table: "projects",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_sub_categories_sub_category_id",
                table: "re_ticket_concerns",
                column: "sub_category_id",
                principalTable: "sub_categories",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_ticket_concerns_ticket_concern_id",
                table: "re_ticket_concerns",
                column: "ticket_concern_id",
                principalTable: "ticket_concerns",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_users_modified_by_user_id",
                table: "re_ticket_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_users_re_ticket_by_user_id",
                table: "re_ticket_concerns",
                column: "re_ticket_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_users_reject_re_ticket_by_user_id",
                table: "re_ticket_concerns",
                column: "reject_re_ticket_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_users_user_id",
                table: "re_ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_re_ticket_concerns_users_user_id1",
                table: "re_ticket_concerns",
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
                name: "fk_receivers_users_user_id",
                table: "receivers",
                column: "added_by",
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
                name: "fk_request_concerns_users_user_id",
                table: "request_concerns",
                column: "added_by",
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
                name: "fk_sub_categories_users_added_by_user_id",
                table: "sub_categories",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sub_categories_users_modified_by_user_id",
                table: "sub_categories",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sub_units_units_unit_id",
                table: "sub_units",
                column: "unit_id",
                principalTable: "units",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_sub_units_users_added_by_user_id",
                table: "sub_units",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_sub_units_users_modified_by_user_id",
                table: "sub_units",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_users_added_by_user_id",
                table: "ticket_attachments",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_attachments_users_modified_by_user_id",
                table: "ticket_attachments",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_ticket_comments_ticket_comment_id",
                table: "ticket_comment_views",
                column: "ticket_comment_id",
                principalTable: "ticket_comments",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_users_user_id",
                table: "ticket_comment_views",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comment_views_users_user_id1",
                table: "ticket_comment_views",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comments_users_added_by_user_id",
                table: "ticket_comments",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_comments_users_modified_by_user_id",
                table: "ticket_comments",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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
                name: "fk_ticket_concerns_users_user_id",
                table: "ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_concerns_users_user_id1",
                table: "ticket_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_users_approver_by_user_id",
                table: "ticket_histories",
                column: "approver_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ticket_histories_users_requestor_by_user_id",
                table: "ticket_histories",
                column: "requestor_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_modified_by_user_id",
                table: "transfer_ticket_concerns",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_reject_transfer_by_user_id",
                table: "transfer_ticket_concerns",
                column: "reject_transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_transfer_by_user_id",
                table: "transfer_ticket_concerns",
                column: "transfer_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_user_id",
                table: "transfer_ticket_concerns",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_transfer_ticket_concerns_users_user_id1",
                table: "transfer_ticket_concerns",
                column: "user_id",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_units_users_added_by_user_id",
                table: "units",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_units_users_modified_by_user_id",
                table: "units",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_users_added_by_user_id",
                table: "user_roles",
                column: "added_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_users_modified_by_user_id",
                table: "user_roles",
                column: "modified_by",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_business_units_users_added_by_user_id",
                table: "business_units");

            migrationBuilder.DropForeignKey(
                name: "fk_business_units_users_modified_by_user_id",
                table: "business_units");

            migrationBuilder.DropForeignKey(
                name: "fk_companies_users_added_by_user_id",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "fk_companies_users_modified_by_user_id",
                table: "companies");

            migrationBuilder.DropForeignKey(
                name: "fk_departments_users_added_by_user_id",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "fk_departments_users_modified_by_user_id",
                table: "departments");

            migrationBuilder.DropForeignKey(
                name: "fk_locations_users_added_by_user_id",
                table: "locations");

            migrationBuilder.DropForeignKey(
                name: "fk_locations_users_modified_by_user_id",
                table: "locations");

            migrationBuilder.DropForeignKey(
                name: "fk_sub_units_users_added_by_user_id",
                table: "sub_units");

            migrationBuilder.DropForeignKey(
                name: "fk_sub_units_users_modified_by_user_id",
                table: "sub_units");

            migrationBuilder.DropForeignKey(
                name: "fk_units_users_added_by_user_id",
                table: "units");

            migrationBuilder.DropForeignKey(
                name: "fk_units_users_modified_by_user_id",
                table: "units");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_users_added_by_user_id",
                table: "user_roles");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_users_modified_by_user_id",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "account_titles");

            migrationBuilder.DropTable(
                name: "approver_ticketings");

            migrationBuilder.DropTable(
                name: "approvers");

            migrationBuilder.DropTable(
                name: "channel_users");

            migrationBuilder.DropTable(
                name: "closing_tickets");

            migrationBuilder.DropTable(
                name: "re_ticket_concerns");

            migrationBuilder.DropTable(
                name: "receivers");

            migrationBuilder.DropTable(
                name: "teams");

            migrationBuilder.DropTable(
                name: "ticket_attachments");

            migrationBuilder.DropTable(
                name: "ticket_comment_views");

            migrationBuilder.DropTable(
                name: "ticket_histories");

            migrationBuilder.DropTable(
                name: "transfer_ticket_concerns");

            migrationBuilder.DropTable(
                name: "ticket_comments");

            migrationBuilder.DropTable(
                name: "ticket_concerns");

            migrationBuilder.DropTable(
                name: "ticket_transactions");

            migrationBuilder.DropTable(
                name: "channels");

            migrationBuilder.DropTable(
                name: "request_concerns");

            migrationBuilder.DropTable(
                name: "sub_categories");

            migrationBuilder.DropTable(
                name: "projects");

            migrationBuilder.DropTable(
                name: "request_transactions");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "locations");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "sub_units");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "departments");

            migrationBuilder.DropTable(
                name: "business_units");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
