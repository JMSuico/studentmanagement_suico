using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<IEnumerable<Attendance>> GetAllAsync();
        Task<Attendance?> GetByIdAsync(int id);
        Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Attendance>> GetByClassSectionIdAsync(int classSectionId);
        Task<IEnumerable<Attendance>> GetByDateRangeAsync(int classSectionId, DateTime startDate, DateTime endDate);
        Task<Attendance?> GetByStudentAndDateAsync(int studentId, int classSectionId, DateTime date);
        Task<Attendance> CreateAsync(Attendance attendance);
        Task UpdateAsync(Attendance attendance);
        Task DeleteAsync(int id);
    }
}
