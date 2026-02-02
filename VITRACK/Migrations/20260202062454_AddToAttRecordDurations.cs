using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VITRACK.Migrations
{
    /// <inheritdoc />
    public partial class AddToAttRecordDurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttendanceDurationMinutes",
                table: "AttendanceRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OvertimeMinutes",
                table: "AttendanceRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PlannedEndTime",
                table: "AttendanceRecords",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PlannedStartTime",
                table: "AttendanceRecords",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlannedWorkingMinutes",
                table: "AttendanceRecords",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceDurationMinutes",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "OvertimeMinutes",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "PlannedEndTime",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "PlannedStartTime",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "PlannedWorkingMinutes",
                table: "AttendanceRecords");
        }
    }
}
