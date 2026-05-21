using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class ClassSectionService : IClassSectionService
    {
        private readonly IClassSectionRepository _repository;

        public ClassSectionService(IClassSectionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ClassSection>> GetAllClassesAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<ClassSection?> GetClassByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<ClassSection> AddClassAsync(ClassSection classSection)
        {
            NormalizeClassTimes(classSection);
            return await _repository.AddAsync(classSection);
        }

        public async Task UpdateClassAsync(ClassSection classSection)
        {
            NormalizeClassTimes(classSection);
            await _repository.UpdateAsync(classSection);
        }

        public async Task DeleteClassAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Normalizes all TimeSpan values on a ClassSection and its child schedules.
        /// Corrects short-form inputs like "9:00" to "09:00:00" to prevent
        /// "not a valid TimeSpan" parsing errors.
        /// </summary>
        private static void NormalizeClassTimes(ClassSection classSection)
        {
            // Normalize legacy StartTime/EndTime on the ClassSection itself
            if (classSection.StartTime.HasValue)
                classSection.StartTime = NormalizeTimeSpan(classSection.StartTime.Value);
            if (classSection.EndTime.HasValue)
                classSection.EndTime = NormalizeTimeSpan(classSection.EndTime.Value);

            // Normalize each child schedule's times
            foreach (var schedule in classSection.Schedules)
            {
                schedule.StartTime = NormalizeTimeSpan(schedule.StartTime);
                schedule.EndTime = NormalizeTimeSpan(schedule.EndTime);
            }
        }

        /// <summary>
        /// Normalizes a TimeSpan to ensure it represents a valid time of day (0–23:59:59).
        /// Converts inputs like "9:00" (which may parse as 9 days) to "09:00:00" (9 hours).
        /// </summary>
        private static TimeSpan NormalizeTimeSpan(TimeSpan time)
        {
            // If the TimeSpan is negative or exceeds 24 hours, clamp to valid range
            if (time.TotalHours < 0)
                return TimeSpan.Zero;
            if (time.TotalHours >= 24)
                return new TimeSpan(time.Hours, time.Minutes, time.Seconds);

            // Ensure clean HH:mm:ss format (strip any extra ticks beyond seconds)
            return new TimeSpan(time.Hours, time.Minutes, time.Seconds);
        }
    }
}
