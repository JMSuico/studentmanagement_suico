using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface ITaskService
    {
        // ClassTask CRUD
        Task<IEnumerable<ClassTask>> GetTasksBySubjectAsync(int subjectId);
        Task<IEnumerable<ClassTask>> GetTasksByCategoryAsync(int subjectId, TaskCategory category);
        Task<ClassTask?> GetTaskByIdAsync(int id);
        Task<ClassTask> CreateTaskAsync(ClassTask classTask);
        Task UpdateTaskAsync(ClassTask classTask);
        Task DeleteTaskAsync(int id);

        // StudentTaskResult CRUD
        Task<IEnumerable<StudentTaskResult>> GetResultsForTaskAsync(int taskId);
        Task<IEnumerable<StudentTaskResult>> GetResultsForStudentAsync(int studentId, int subjectId);
        Task<StudentTaskResult> SaveStudentResultAsync(StudentTaskResult result);
    }
}
