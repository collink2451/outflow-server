using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Outflow.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixFrequencyTypo2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 5,
                column: "Name",
                value: "Annually");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Frequencies",
                keyColumn: "FrequencyId",
                keyValue: 5,
                column: "Name",
                value: "Anually");
        }
    }
}
