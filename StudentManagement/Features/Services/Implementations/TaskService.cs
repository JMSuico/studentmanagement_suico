using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Services.Interfaces;
using StudentManagement.Features.Repositories.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;

        public TaskService(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<ClassTask>> GetTasksBySubjectAsync(int subjectId)
        {
            return await _taskRepository.GetTasksBySubjectAsync(subjectId);
        }

        public async Task<IEnumerable<ClassTask>> GetTasksByCategoryAsync(int subjectId, TaskCategory category)
        {
            return await _taskRepository.GetTasksByCategoryAsync(subjectId, category);
        }

        public async Task<ClassTask?> GetTaskByIdAsync(int id)
        {
            return await _taskRepository.GetByIdAsync(id);
        }

        public async Task<ClassTask> CreateTaskAsync(ClassTask classTask)
        {
            return await _taskRepository.CreateAsync(classTask);
        }

        public async Task UpdateTaskAsync(ClassTask classTask)
        {
            await _taskRepository.UpdateAsync(classTask);
        }

        public async Task DeleteTaskAsync(int id)
        {
            await _taskRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<StudentTaskResult>> GetResultsForTaskAsync(int taskId)
        {
            return await _taskRepository.GetResultsForTaskAsync(taskId);
        }

        public async Task<IEnumerable<StudentTaskResult>> GetResultsForStudentAsync(int studentId, int subjectId)
        {
            return await _taskRepository.GetResultsForStudentAsync(studentId, subjectId);
        }

        public async Task<StudentTaskResult> SaveStudentResultAsync(StudentTaskResult result)
        {
            var existing = await _taskRepository.GetStudentResultAsync(result.ClassTaskId, result.StudentId);

            if (existing != null)
            {
                existing.PointsEarned = result.PointsEarned;
                existing.ComputedScore = result.ComputedScore;
                existing.Status = result.Status;
                existing.Remarks = result.Remarks;
                existing.SubmittedAt = result.SubmittedAt;
                
                await _taskRepository.UpdateResultAsync(existing);
                return existing;
            }
            else
            {
                return await _taskRepository.CreateResultAsync(result);
            }
        }
    }
}
