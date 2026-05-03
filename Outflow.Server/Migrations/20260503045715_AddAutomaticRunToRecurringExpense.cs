using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outflow.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomaticRunToRecurringExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutomaticRun",
                table: "RecurringExpenses",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutomaticRun",
                table: "RecurringExpenses");
        }
    }
}
