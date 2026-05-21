using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface IAttendanceSessionRepository
    {
        Task<IEnumerable<AttendanceSession>> GetByClassSectionIdAsync(int classSectionId);
        Task<AttendanceSession?> GetActiveSessionAsync(int classSectionId, DateTime date);
        Task<IEnumerable<AttendanceSession>> GetActiveSessionsForClassDateAsync(int classSectionId, DateTime date);
        Task<AttendanceSession> CreateAsync(AttendanceSession session);
        Task UpdateMultipleAsync(IEnumerable<AttendanceSession> sessions);
        Task<AttendanceSession?> GetActiveSessionByCodeAsync(string code);
    }
}