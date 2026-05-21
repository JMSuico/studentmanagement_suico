using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Teacher>> GetAllAsync();
        Task<Teacher?> GetByIdAsync(int id);
        Task<Teacher?> GetByEmployeeNumberAsync(string employeeNumber);
        Task<Teacher> CreateAsync(Teacher teacher);
        Task UpdateAsync(Teacher teacher);
        Task DeleteAsync(int id);
    }
}
