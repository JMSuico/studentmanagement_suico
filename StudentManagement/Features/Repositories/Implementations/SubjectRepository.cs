using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly AppDbContext _context;

        public SubjectRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.Subjects.AsNoTracking().ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.Subjects.FindAsync(id);
        }

        public async Task<Subject?> GetByCodeAsync(string subjectCode)
        {
            return await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectCode == subjectCode);
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                // Get all class sections taught for this subject
                var classes = await _context.ClassSections
                    .Include(c => c.Schedules)
                    .Include(c => c.Enrollments)
                    .Include(c => c.Attendances)
                    .Include(c => c.Grades)
                    .Where(c => c.SubjectId == id)
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

                // Also delete all tasks, task results, and grade setups for this subject
                var tasks = await _context.ClassTasks.Where(t => t.SubjectId == id).ToListAsync();
                foreach (var task in tasks)
                {
                    var results = await _context.StudentTaskResults.Where(r => r.ClassTaskId == task.Id).ToListAsync();
                    _context.StudentTaskResults.RemoveRange(results);
                }
                _context.ClassTasks.RemoveRange(tasks);

                var setups = await _context.SubjectGradeSetups.Where(s => s.SubjectId == id).ToListAsync();
                _context.SubjectGradeSetups.RemoveRange(setups);

                // Also delete any grades directly associated with this subject
                var grades = await _context.Grades.Where(g => g.SubjectId == id).ToListAsync();
                _context.Grades.RemoveRange(grades);

                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }
        }
    }
}
