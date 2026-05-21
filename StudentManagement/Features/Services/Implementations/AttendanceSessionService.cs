using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Services.Interfaces;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class AttendanceSessionService : IAttendanceSessionService
    {
        private readonly IAttendanceSessionRepository _attendanceSessionRepository;

        public AttendanceSessionService(IAttendanceSessionRepository attendanceSessionRepository)
        {
            _attendanceSessionRepository = attendanceSessionRepository;
        }

        public async Task<IEnumerable<AttendanceSession>> GetSessionsForClassAsync(int classSectionId)
        {
            return await _attendanceSessionRepository.GetByClassSectionIdAsync(classSectionId);
        }

        public async Task<AttendanceSession?> GetActiveSessionAsync(int classSectionId, DateTime date)
        {
            return await _attendanceSessionRepository.GetActiveSessionAsync(classSectionId, date);
        }

        public async Task<AttendanceSession> GenerateSessionAsync(int classSectionId, DateTime date)
        {
            // Deactivate existing sessions for this date/class
            var existingSessions = await _attendanceSessionRepository.GetActiveSessionsForClassDateAsync(classSectionId, date);

            foreach (var session in existingSessions)
            {
                session.IsActive = false;
            }
            if (existingSessions.Any())
            {
                await _attendanceSessionRepository.UpdateMultipleAsync(existingSessions);
            }

            // Generate new 6-digit code
            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            var newSession = new AttendanceSession
            {
                ClassSectionId = classSectionId,
                Date = date.Date,
                GeneratedCode = code,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _attendanceSessionRepository.CreateAsync(newSession);
        }

        public async Task<AttendanceSession?> ValidateAndGetSessionAsync(string code)
        {
            return await _attendanceSessionRepository.GetActiveSessionByCodeAsync(code);
        }
    }
}
