using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outflow.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddVendorCategoryNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaidTransactions_PlaidConnections_PlaidConnectionId",
                table: "PlaidTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "ExpenseCategoryId",
                table: "Vendors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaidTransactions_PlaidConnections_PlaidConnectionId",
                table: "PlaidTransactions",
                column: "PlaidConnectionId",
                principalTable: "PlaidConnections",
                principalColumn: "PlaidConnectionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaidTransactions_PlaidConnections_PlaidConnectionId",
                table: "PlaidTransactions");

            migrationBuilder.AlterColumn<int>(
                name: "ExpenseCategoryId",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaidTransactions_PlaidConnections_PlaidConnectionId",
                table: "PlaidTransactions",
                column: "PlaidConnectionId",
                principalTable: "PlaidConnections",
                principalColumn: "PlaidConnectionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
