using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _subjectRepository;

        public SubjectService(ISubjectRepository subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IEnumerable<Subject>> GetAllSubjectsAsync()
        {
            return await _subjectRepository.GetAllAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(int id)
        {
            return await _subjectRepository.GetByIdAsync(id);
        }

        public async Task<Subject?> GetSubjectByCodeAsync(string subjectCode)
        {
            return await _subjectRepository.GetByCodeAsync(subjectCode);
        }

        public async Task<Subject> CreateSubjectAsync(Subject subject)
        {
            var existing = await _subjectRepository.GetByCodeAsync(subjectCode: subject.SubjectCode);
            if (existing != null)
                throw new Exception($"Subject with code {subject.SubjectCode} already exists.");

            subject.CreatedAt = DateTime.UtcNow;
            return await _subjectRepository.CreateAsync(subject);
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            await _subjectRepository.UpdateAsync(subject);
        }

        public async Task DeleteSubjectAsync(int id)
        {
            await _subjectRepository.DeleteAsync(id);
        }
    }
}
