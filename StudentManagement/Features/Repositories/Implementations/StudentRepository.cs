using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            return await _context.Students
                .Include(s => s.Guardian)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Guardian)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Student?> GetByStudentNumberAsync(string studentNumber)
        {
            return await _context.Students
                .Include(s => s.Guardian)
                .FirstOrDefaultAsync(s => s.StudentNumber == studentNumber);
        }

        public async Task<IEnumerable<Student>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            searchTerm = searchTerm.ToLower();
            return await _context.Students
                .Include(s => s.Guardian)
                .Where(s => s.FirstName.ToLower().Contains(searchTerm) || 
                            s.LastName.ToLower().Contains(searchTerm) || 
                            s.StudentNumber.ToLower().Contains(searchTerm))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student> CreateAsync(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task UpdateAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
        }
    }
}
