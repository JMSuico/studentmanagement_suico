using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student?> GetStudentByNumberAsync(string studentNumber);
        Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm);
        Task<Student> CreateStudentAsync(Student student);
        Task UpdateStudentAsync(Student student);
        Task PromoteAllStudentsAsync();
        Task RevertPromoteAllStudentsAsync();
        Task DeleteStudentAsync(int id);
    }
}
