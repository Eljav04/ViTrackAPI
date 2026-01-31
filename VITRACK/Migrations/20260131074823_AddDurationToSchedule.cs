using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VITRACK.Migrations
{
    /// <inheritdoc />
    public partial class AddDurationToSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "WorkSchedules",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "WorkSchedules");
        }
    }
}
