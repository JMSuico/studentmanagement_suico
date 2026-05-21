using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _context;

        public TaskRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ClassTask>> GetTasksBySubjectAsync(int subjectId)
        {
            return await _context.ClassTasks
                .Include(t => t.Subject)
                .Where(t => t.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClassTask>> GetTasksByCategoryAsync(int subjectId, TaskCategory category)
        {
            return await _context.ClassTasks
                .Include(t => t.Subject)
                .Where(t => t.SubjectId == subjectId && t.Category == category)
                .ToListAsync();
        }

        public async Task<ClassTask?> GetByIdAsync(int id)
        {
            return await _context.ClassTasks
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<ClassTask> CreateAsync(ClassTask classTask)
        {
            _context.ClassTasks.Add(classTask);
            await _context.SaveChangesAsync();
            return classTask;
        }

        public async Task UpdateAsync(ClassTask classTask)
        {
            _context.ClassTasks.Update(classTask);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var task = await _context.ClassTasks.FindAsync(id);
            if (task != null)
            {
                _context.ClassTasks.Remove(task);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StudentTaskResult>> GetResultsForTaskAsync(int taskId)
        {
            return await _context.StudentTaskResults
                .Include(r => r.Student)
                .Include(r => r.ClassTask)
                .Where(r => r.ClassTaskId == taskId)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentTaskResult>> GetResultsForStudentAsync(int studentId, int subjectId)
        {
            return await _context.StudentTaskResults
                .Include(r => r.ClassTask)
                .Where(r => r.StudentId == studentId && r.ClassTask!.SubjectId == subjectId)
                .ToListAsync();
        }

        public async Task<StudentTaskResult?> GetStudentResultAsync(int classTaskId, int studentId)
        {
            return await _context.StudentTaskResults
                .FirstOrDefaultAsync(r => r.ClassTaskId == classTaskId && r.StudentId == studentId);
        }

        public async Task<StudentTaskResult> CreateResultAsync(StudentTaskResult result)
        {
            _context.StudentTaskResults.Add(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async Task UpdateResultAsync(StudentTaskResult result)
        {
            _context.StudentTaskResults.Update(result);
            await _context.SaveChangesAsync();
        }
    }
}
