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
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly AppDbContext _context;

        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAllAsync()
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.ClassSection)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Attendance?> GetByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.ClassSection)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Attendance>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Attendances
                .Include(a => a.ClassSection)
                .Where(a => a.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetByClassSectionIdAsync(int classSectionId)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.ClassSectionId == classSectionId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(int classSectionId, DateTime startDate, DateTime endDate)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Where(a => a.ClassSectionId == classSectionId && a.Date >= startDate && a.Date <= endDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Attendance?> GetByStudentAndDateAsync(int studentId, int classSectionId, DateTime date)
        {
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentId == studentId && a.ClassSectionId == classSectionId && a.Date.Date == date.Date);
        }

        public async Task<Attendance> CreateAsync(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance != null)
            {
                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
            }
        }
    }
}
