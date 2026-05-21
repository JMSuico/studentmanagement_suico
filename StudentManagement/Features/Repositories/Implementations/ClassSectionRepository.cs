using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class ClassSectionRepository : IClassSectionRepository
    {
        private readonly AppDbContext _context;

        public ClassSectionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassSection>> GetAllAsync()
        {
            return await _context.ClassSections
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .Include(c => c.Schedules)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ClassSection?> GetByIdAsync(int id)
        {
            return await _context.ClassSections
                .Include(c => c.Subject)
                .Include(c => c.Teacher)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ClassSection> AddAsync(ClassSection classSection)
        {
            _context.ClassSections.Add(classSection);
            await _context.SaveChangesAsync();
            return classSection;
        }

        public async Task UpdateAsync(ClassSection classSection)
        {
            // Load existing entity with its schedules for proper tracking
            var existing = await _context.ClassSections
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Id == classSection.Id);

            if (existing == null) return;

            // Update scalar properties
            _context.Entry(existing).CurrentValues.SetValues(classSection);

            // Replace schedules: remove old, add new
            _context.ClassSchedules.RemoveRange(existing.Schedules);
            foreach (var schedule in classSection.Schedules)
            {
                schedule.Id = 0; // Reset ID so EF creates new rows
                schedule.ClassSectionId = classSection.Id;
                existing.Schedules.Add(schedule);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _context.ClassSections
                .Include(c => c.Schedules)
                .Include(c => c.Enrollments)
                .Include(c => c.Attendances)
                .Include(c => c.Grades)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (entity != null)
            {
                // Remove related entities first
                _context.ClassSchedules.RemoveRange(entity.Schedules);
                _context.Enrollments.RemoveRange(entity.Enrollments);
                _context.Attendances.RemoveRange(entity.Attendances);
                _context.Grades.RemoveRange(entity.Grades);
                
                // Remove related AttendanceSessions
                var sessions = await _context.AttendanceSessions.Where(s => s.ClassSectionId == id).ToListAsync();
                _context.AttendanceSessions.RemoveRange(sessions);

                // Remove the ClassSection itself
                _context.ClassSections.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
