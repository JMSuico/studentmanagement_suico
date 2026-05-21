using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface IClassSectionRepository
    {
        Task<IEnumerable<ClassSection>> GetAllAsync();
        Task<ClassSection?> GetByIdAsync(int id);
        Task<ClassSection> AddAsync(ClassSection classSection);
        Task UpdateAsync(ClassSection classSection);
        Task DeleteAsync(int id);
    }
}
