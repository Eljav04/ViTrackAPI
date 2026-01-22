using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VITRACK.Migrations
{
    /// <inheritdoc />
    public partial class AttendacesRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ArrivalTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    LeaveTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ArrivalLongitude = table.Column<double>(type: "float", nullable: true),
                    ArrivalLatitude = table.Column<double>(type: "float", nullable: true),
                    LeaveLongitude = table.Column<double>(type: "float", nullable: true),
                    LeaveLatitude = table.Column<double>(type: "float", nullable: true),
                    ArrivalImgUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LeaveImgUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    LateReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    EarlyLeaveReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    QrApprovedArrival = table.Column<bool>(type: "bit", nullable: false),
                    QrApprovedLeave = table.Column<bool>(type: "bit", nullable: false),
                    IsLate = table.Column<bool>(type: "bit", nullable: false),
                    IsEarlyLeave = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceRecords_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_EmployeeId",
                table: "AttendanceRecords",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceRecords");
        }
    }
}
