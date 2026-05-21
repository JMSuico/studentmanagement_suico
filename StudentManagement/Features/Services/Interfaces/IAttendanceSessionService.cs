using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IAttendanceSessionService
    {
        Task<IEnumerable<AttendanceSession>> GetSessionsForClassAsync(int classSectionId);
        Task<AttendanceSession?> GetActiveSessionAsync(int classSectionId, DateTime date);
        Task<AttendanceSession> GenerateSessionAsync(int classSectionId, DateTime date);
        Task<AttendanceSession?> ValidateAndGetSessionAsync(string code);
    }
}
