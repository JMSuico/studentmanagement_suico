using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<Attendance?> GetAttendanceByIdAsync(int id);
        Task<IEnumerable<Attendance>> GetStudentAttendancesAsync(int studentId);
        Task<IEnumerable<Attendance>> GetClassSectionAttendancesAsync(int classSectionId);
        Task<IEnumerable<Attendance>> GetClassAttendancesByDateRangeAsync(int classSectionId, DateTime startDate, DateTime endDate);
        Task<Attendance> RecordAttendanceAsync(int studentId, int classSectionId, DateTime date, AttendanceStatus status, TimeSpan? timeIn = null, TimeSpan? timeOut = null, string? remarks = null);
        Task UpdateAttendanceAsync(Attendance attendance);
        Task DeleteAttendanceAsync(int id);
        
        Task<Attendance> SubmitAttendanceCodeAsync(int studentId, string code);
    }
}
