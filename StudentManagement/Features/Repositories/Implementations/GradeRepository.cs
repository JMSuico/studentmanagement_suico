using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class GradeRepository : IGradeRepository
    {
        private readonly AppDbContext _context;

        public GradeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Grade>> GetAllAsync()
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Include(g => g.ClassSection)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Grade?> GetByIdAsync(int id)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Include(g => g.ClassSection)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<Grade>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Grades
                .Include(g => g.Subject)
                .Include(g => g.ClassSection)
                .Where(g => g.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Grade>> GetByClassSectionIdAsync(int classSectionId)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Subject)
                .Where(g => g.ClassSectionId == classSectionId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Grade?> GetByStudentAndSubjectAsync(int studentId, int classSectionId, int subjectId)
        {
            return await _context.Grades
                .FirstOrDefaultAsync(g => g.StudentId == studentId && g.ClassSectionId == classSectionId && g.SubjectId == subjectId);
        }

        public async Task<Grade> CreateAsync(Grade grade)
        {
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task UpdateAsync(Grade grade)
        {
            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<SubjectGradeSetup>> GetAllGradeSetupsAsync()
        {
            return await _context.SubjectGradeSetups
                .Include(s => s.Subject)
                .ToListAsync();
        }

        public async Task<SubjectGradeSetup?> GetGradeSetupAsync(int subjectId, string semester)
        {
            return await _context.SubjectGradeSetups
                .FirstOrDefaultAsync(s => s.SubjectId == subjectId && s.Semester == semester);
        }

        public async Task<SubjectGradeSetup> CreateGradeSetupAsync(SubjectGradeSetup setup)
        {
            _context.SubjectGradeSetups.Add(setup);
            await _context.SaveChangesAsync();
            return setup;
        }

        public async Task UpdateGradeSetupAsync(SubjectGradeSetup setup)
        {
            _context.SubjectGradeSetups.Update(setup);
            await _context.SaveChangesAsync();
        }
    }
}
