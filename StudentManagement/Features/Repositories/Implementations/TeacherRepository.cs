using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly AppDbContext _context;

        public TeacherRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllAsync()
        {
            return await _context.Teachers.AsNoTracking().ToListAsync();
        }

        public async Task<Teacher?> GetByIdAsync(int id)
        {
            return await _context.Teachers.FindAsync(id);
        }

        public async Task<Teacher?> GetByEmployeeNumberAsync(string employeeNumber)
        {
            return await _context.Teachers.FirstOrDefaultAsync(t => t.EmployeeNumber == employeeNumber);
        }

        public async Task<Teacher> CreateAsync(Teacher teacher)
        {
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();
            return teacher;
        }

        public async Task UpdateAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher != null)
            {
                // Get all class sections taught by this teacher
                var classes = await _context.ClassSections
                    .Include(c => c.Schedules)
                    .Include(c => c.Enrollments)
                    .Include(c => c.Attendances)
                    .Include(c => c.Grades)
                    .Where(c => c.TeacherId == id)
                    .ToListAsync();

                foreach (var classSection in classes)
                {
                    _context.ClassSchedules.RemoveRange(classSection.Schedules);
                    _context.Enrollments.RemoveRange(classSection.Enrollments);
                    _context.Attendances.RemoveRange(classSection.Attendances);
                    _context.Grades.RemoveRange(classSection.Grades);
                    
                    var sessions = await _context.AttendanceSessions.Where(s => s.ClassSectionId == classSection.Id).ToListAsync();
                    _context.AttendanceSessions.RemoveRange(sessions);
                }

                _context.ClassSections.RemoveRange(classes);
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }
        }
    }
}
