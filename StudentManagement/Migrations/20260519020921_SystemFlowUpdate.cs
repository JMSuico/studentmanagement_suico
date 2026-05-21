using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentManagement.Migrations
{
    /// <inheritdoc />
    public partial class SystemFlowUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DaysOfWeek",
                table: "ClassSections",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "ClassSections",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "ClassSections",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AttendanceSessionId",
                table: "Attendances",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodeUsed",
                table: "Attendances",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeSubmitted",
                table: "Attendances",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AttendanceSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassSectionId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    GeneratedCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceSessions_ClassSections_ClassSectionId",
                        column: x => x.ClassSectionId,
                        principalTable: "ClassSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClassTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaxPoints = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassTasks_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubjectGradeSetups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuizzesPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    LongExamPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PerformancePercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    SemesterExamPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    HandsOnPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PracticalPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OralPercentage = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectGradeSetups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectGradeSetups_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudentTaskResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClassTaskId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    PointsEarned = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ComputedScore = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubmittedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTaskResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentTaskResults_ClassTasks_ClassTaskId",
                        column: x => x.ClassTaskId,
                        principalTable: "ClassTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentTaskResults_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_AttendanceSessionId",
                table: "Attendances",
                column: "AttendanceSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceSessions_ClassSectionId",
                table: "AttendanceSessions",
                column: "ClassSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTasks_SubjectId",
                table: "ClassTasks",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentTaskResults_ClassTaskId_StudentId",
                table: "StudentTaskResults",
                columns: new[] { "ClassTaskId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentTaskResults_StudentId",
                table: "StudentTaskResults",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGradeSetups_SubjectId_Semester",
                table: "SubjectGradeSetups",
                columns: new[] { "SubjectId", "Semester" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AttendanceSessions_AttendanceSessionId",
                table: "Attendances",
                column: "AttendanceSessionId",
                principalTable: "AttendanceSessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AttendanceSessions_AttendanceSessionId",
                table: "Attendances");

            migrationBuilder.DropTable(
                name: "AttendanceSessions");

            migrationBuilder.DropTable(
                name: "StudentTaskResults");

            migrationBuilder.DropTable(
                name: "SubjectGradeSetups");

            migrationBuilder.DropTable(
                name: "ClassTasks");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_AttendanceSessionId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "DaysOfWeek",
                table: "ClassSections");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ClassSections");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "ClassSections");

            migrationBuilder.DropColumn(
                name: "AttendanceSessionId",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "CodeUsed",
                table: "Attendances");

            migrationBuilder.DropColumn(
                name: "TimeSubmitted",
                table: "Attendances");
        }
    }
}
