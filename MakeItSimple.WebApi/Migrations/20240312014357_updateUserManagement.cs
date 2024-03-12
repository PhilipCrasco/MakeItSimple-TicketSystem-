using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MakeItSimple.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class updateUserManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "permissions",
                value: "[\"Overview\",\"User Management\",\"User Role\",\"User Account\",\"Request\",\"Channel\",\"Filing\",\"Generate\",\"Masterlist\",\"Company\",\"Business Unit\",\"Unit\",\"Location\",\"Sub Unit\",\"Department\",\"Category\",\"Sub Category\"]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "user_roles",
                keyColumn: "id",
                keyValue: 1,
                column: "permissions",
                value: "[\"User Management\",\"User Role\",\"User Account\",\"Masterlist\",\"Company\",\"Department\",\"Request\",\"Channel\",\"Filing\",\"Generate\"]");
        }
    }
}
