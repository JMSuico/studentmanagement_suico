using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        Task<IEnumerable<ClassTask>> GetTasksBySubjectAsync(int subjectId);
        Task<IEnumerable<ClassTask>> GetTasksByCategoryAsync(int subjectId, TaskCategory category);
        Task<ClassTask?> GetByIdAsync(int id);
        Task<ClassTask> CreateAsync(ClassTask classTask);
        Task UpdateAsync(ClassTask classTask);
        Task DeleteAsync(int id);

        Task<IEnumerable<StudentTaskResult>> GetResultsForTaskAsync(int taskId);
        Task<IEnumerable<StudentTaskResult>> GetResultsForStudentAsync(int studentId, int subjectId);
        Task<StudentTaskResult?> GetStudentResultAsync(int classTaskId, int studentId);
        Task<StudentTaskResult> CreateResultAsync(StudentTaskResult result);
        Task UpdateResultAsync(StudentTaskResult result);
    }
}
