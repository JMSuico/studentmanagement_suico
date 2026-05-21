using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IClassSectionService
    {
        Task<IEnumerable<ClassSection>> GetAllClassesAsync();
        Task<ClassSection?> GetClassByIdAsync(int id);
        Task<ClassSection> AddClassAsync(ClassSection classSection);
        Task UpdateClassAsync(ClassSection classSection);
        Task DeleteClassAsync(int id);
    }
}
