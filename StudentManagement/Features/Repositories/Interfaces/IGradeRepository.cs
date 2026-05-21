using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task<IEnumerable<Grade>> GetAllAsync();
        Task<Grade?> GetByIdAsync(int id);
        Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Grade>> GetByClassSectionIdAsync(int classSectionId);
        Task<Grade?> GetByStudentAndSubjectAsync(int studentId, int classSectionId, int subjectId);
        Task<Grade> CreateAsync(Grade grade);
        Task UpdateAsync(Grade grade);
        Task DeleteAsync(int id);

        Task<IEnumerable<SubjectGradeSetup>> GetAllGradeSetupsAsync();
        Task<SubjectGradeSetup?> GetGradeSetupAsync(int subjectId, string semester);
        Task<SubjectGradeSetup> CreateGradeSetupAsync(SubjectGradeSetup setup);
        Task UpdateGradeSetupAsync(SubjectGradeSetup setup);
    }
}
