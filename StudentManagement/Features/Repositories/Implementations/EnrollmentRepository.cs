using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly AppDbContext _context;

        public EnrollmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassSection)
                    .ThenInclude(c => c!.Subject)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Enrollment?> GetByIdAsync(int id)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassSection)
                    .ThenInclude(c => c!.Subject)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.ClassSection)
                    .ThenInclude(c => c!.Subject)
                .Where(e => e.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByClassSectionIdAsync(int classSectionId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Where(e => e.ClassSectionId == classSectionId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Enrollment> CreateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment != null)
            {
                _context.Enrollments.Remove(enrollment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int studentId, int classSectionId)
        {
            return await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.ClassSectionId == classSectionId);
        }
    }
}
