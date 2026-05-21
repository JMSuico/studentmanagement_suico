using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject?> GetByCodeAsync(string subjectCode);
        Task<Subject> CreateAsync(Subject subject);
        Task UpdateAsync(Subject subject);
        Task DeleteAsync(int id);
    }
}
