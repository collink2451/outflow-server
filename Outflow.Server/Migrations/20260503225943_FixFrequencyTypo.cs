using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outflow.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixFrequencyTypo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 4,
                column: "Name",
                value: "Semi-Annually");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 4,
                column: "Name",
                value: "Semi-Annualy");
        }
    }
}
