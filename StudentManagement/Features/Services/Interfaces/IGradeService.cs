using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IGradeService
    {
        Task<IEnumerable<Grade>> GetAllGradesAsync();
        Task<Grade?> GetGradeByIdAsync(int id);
        Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId);
        Task<IEnumerable<Grade>> GetClassSectionGradesAsync(int classSectionId);
        Task<Grade> SubmitGradeAsync(int studentId, int classSectionId, int subjectId, decimal? prelim, decimal? midterm, decimal? final, string? remarks = null);
        Task UpdateGradeAsync(Grade grade);
        Task ApproveGradeAsync(int gradeId);
        Task ReleaseGradeAsync(int gradeId);
        Task DeleteGradeAsync(int id);

        // Grade Setup
        Task<IEnumerable<SubjectGradeSetup>> GetAllGradeSetupsAsync();
        Task<SubjectGradeSetup?> GetGradeSetupAsync(int subjectId, string semester);
        Task<SubjectGradeSetup> SaveGradeSetupAsync(SubjectGradeSetup setup);
        
        // Auto-computation
        Task RecalculateStudentGradeAsync(int studentId, int subjectId, string semester);
    }
}
