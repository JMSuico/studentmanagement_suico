using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class AttendanceSessionRepository : IAttendanceSessionRepository
    {
        private readonly AppDbContext _context;

        public AttendanceSessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceSession>> GetByClassSectionIdAsync(int classSectionId)
        {
            return await _context.AttendanceSessions
                .Include(s => s.ClassSection)
                .Where(s => s.ClassSectionId == classSectionId)
                .OrderByDescending(s => s.Date)
                .ThenByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<AttendanceSession?> GetActiveSessionAsync(int classSectionId, DateTime date)
        {
            return await _context.AttendanceSessions
                .FirstOrDefaultAsync(s => s.ClassSectionId == classSectionId && s.Date.Date == date.Date && s.IsActive);
        }

        public async Task<IEnumerable<AttendanceSession>> GetActiveSessionsForClassDateAsync(int classSectionId, DateTime date)
        {
            return await _context.AttendanceSessions
                .Where(s => s.ClassSectionId == classSectionId && s.Date.Date == date.Date && s.IsActive)
                .ToListAsync();
        }

        public async Task<AttendanceSession> CreateAsync(AttendanceSession session)
        {
            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task UpdateMultipleAsync(IEnumerable<AttendanceSession> sessions)
        {
            _context.AttendanceSessions.UpdateRange(sessions);
            await _context.SaveChangesAsync();
        }

        public async Task<AttendanceSession?> GetActiveSessionByCodeAsync(string code)
        {
            return await _context.AttendanceSessions
                .Include(s => s.ClassSection)
                .FirstOrDefaultAsync(s => s.GeneratedCode == code && s.IsActive);
        }
    }
}