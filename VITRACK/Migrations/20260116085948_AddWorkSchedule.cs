using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VITRACK.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkScheduleId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WorkSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSchedules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_WorkScheduleId",
                table: "AspNetUsers",
                column: "WorkScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_WorkSchedules_WorkScheduleId",
                table: "AspNetUsers",
                column: "WorkScheduleId",
                principalTable: "WorkSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_WorkSchedules_WorkScheduleId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_WorkScheduleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WorkScheduleId",
                table: "AspNetUsers");
        }
    }
}
