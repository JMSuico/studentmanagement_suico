using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Features.Services.Interfaces
{
    public class ProgressSummary
    {
        public int StudentId { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string YearLevel { get; set; } = string.Empty;
        public string Section { get; set; } = string.Empty;
        public double AttendanceRate { get; set; }
        public double GradeAverage { get; set; }
        public int PassedSubjects { get; set; }
        public int FailedSubjects { get; set; }
        public string Standing { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }

    public interface IAcademicProgressService
    {
        Task<IEnumerable<ProgressSummary>> GetAllProgressSummariesAsync();
        Task<ProgressSummary?> GetStudentProgressAsync(int studentId);
    }
}
