using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class AcademicProgressService : IAcademicProgressService
    {
        private readonly IStudentService _studentService;
        private readonly IAttendanceService _attendanceService;
        private readonly IGradeService _gradeService;

        public AcademicProgressService(
            IStudentService studentService,
            IAttendanceService attendanceService,
            IGradeService gradeService)
        {
            _studentService = studentService;
            _attendanceService = attendanceService;
            _gradeService = gradeService;
        }

        public async Task<IEnumerable<ProgressSummary>> GetAllProgressSummariesAsync()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var summaries = new List<ProgressSummary>();

            foreach (var student in students)
            {
                var summary = await BuildProgressSummary(student.Id, student.StudentNumber,
                    $"{student.FirstName} {student.LastName}",
                    student.YearLevel.ToString(),
                    student.Section ?? "");
                summaries.Add(summary);
            }

            return summaries;
        }

        public async Task<ProgressSummary?> GetStudentProgressAsync(int studentId)
        {
            var students = await _studentService.GetAllStudentsAsync();
            var student = students.FirstOrDefault(s => s.Id == studentId);
            if (student == null) return null;

            return await BuildProgressSummary(student.Id, student.StudentNumber,
                $"{student.FirstName} {student.LastName}",
                student.YearLevel.ToString(),
                student.Section ?? "");
        }

        private async Task<ProgressSummary> BuildProgressSummary(
            int studentId, string studentNumber, string fullName, string yearLevel, string section)
        {
            // Calculate attendance rate
            var attendances = (await _attendanceService.GetStudentAttendancesAsync(studentId)).ToList();
            double attendanceRate = 0;
            if (attendances.Count > 0)
            {
                var presentOrLate = attendances.Count(a =>
                    a.Status == AttendanceStatus.Present ||
                    a.Status == AttendanceStatus.Late ||
                    a.Status == AttendanceStatus.Excused);
                attendanceRate = (double)presentOrLate / attendances.Count * 100.0;
            }

            // Calculate grade average and passed/failed
            var grades = (await _gradeService.GetStudentGradesAsync(studentId)).ToList();
            double gradeAverage = 0;
            int passed = 0;
            int failed = 0;

            var gradesWithOverall = grades.Where(g => g.OverallGrade.HasValue).ToList();
            if (gradesWithOverall.Count > 0)
            {
                gradeAverage = (double)gradesWithOverall.Average(g => g.OverallGrade!.Value);
                passed = gradesWithOverall.Count(g => g.OverallGrade!.Value >= 75);
                failed = gradesWithOverall.Count(g => g.OverallGrade!.Value < 75);
            }

            // Determine academic standing
            string standing;
            if (gradeAverage >= 85 && failed == 0)
                standing = "Excellent";
            else if (gradeAverage >= 75 && failed == 0)
                standing = "Good";
            else if (failed > 0)
                standing = "Needs Improvement";
            else
                standing = "No Data";

            // Generate remarks
            string remarks;
            if (standing == "Excellent")
                remarks = "Outstanding academic performance. Keep up the great work!";
            else if (standing == "Good")
                remarks = "Good academic standing. Continue working hard to reach excellence.";
            else if (standing == "Needs Improvement")
                remarks = $"Has {failed} failed subject(s). Additional effort and support needed.";
            else
                remarks = "No grade records available yet.";

            if (attendanceRate < 80 && attendances.Count > 0)
                remarks += " Attendance needs improvement.";

            return new ProgressSummary
            {
                StudentId = studentId,
                StudentNumber = studentNumber,
                FullName = fullName,
                YearLevel = yearLevel,
                Section = section,
                AttendanceRate = Math.Round(attendanceRate, 1),
                GradeAverage = Math.Round(gradeAverage, 2),
                PassedSubjects = passed,
                FailedSubjects = failed,
                Standing = standing,
                Remarks = remarks
            };
        }
    }
}
