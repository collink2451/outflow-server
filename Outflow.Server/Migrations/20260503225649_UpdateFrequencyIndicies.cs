using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outflow.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFrequencyIndicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 2,
                column: "Name",
                value: "Bi-Weekly");

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 4,
                columns: new[] { "MonthsInterval", "Name" },
                values: new object[] { 6, "Semi-Annualy" });

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 5,
                columns: new[] { "MonthsInterval", "Name" },
                values: new object[] { 12, "Anually" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 2,
                column: "Name",
                value: "BiWeekly");

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 4,
                columns: new[] { "MonthsInterval", "Name" },
                values: new object[] { 12, "Yearly" });

            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 5,
                columns: new[] { "MonthsInterval", "Name" },
                values: new object[] { 6, "BiYearly" });
        }
    }
}
