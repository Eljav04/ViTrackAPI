using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VITRACK.Migrations
{
    /// <inheritdoc />
    public partial class AddToAttRecordIsAbsentIsRest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAbsent",
                table: "AttendanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRest",
                table: "AttendanceRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAbsent",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "IsRest",
                table: "AttendanceRecords");
        }
    }
}
