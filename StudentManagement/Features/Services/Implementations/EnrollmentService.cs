using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepository;

        public EnrollmentService(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        public async Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync()
        {
            return await _enrollmentRepository.GetAllAsync();
        }

        public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _enrollmentRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(int studentId)
        {
            return await _enrollmentRepository.GetByStudentIdAsync(studentId);
        }

        public async Task<IEnumerable<Enrollment>> GetClassSectionEnrollmentsAsync(int classSectionId)
        {
            return await _enrollmentRepository.GetByClassSectionIdAsync(classSectionId);
        }

        public async Task<Enrollment> EnrollStudentAsync(int studentId, int classSectionId, string? remarks = null, EnrollmentStatus status = EnrollmentStatus.Enrolled)
        {
            if (await _enrollmentRepository.ExistsAsync(studentId, classSectionId))
                throw new Exception("Student is already enrolled in this class section.");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                ClassSectionId = classSectionId,
                Status = status,
                Remarks = remarks,
                EnrollmentDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            return await _enrollmentRepository.CreateAsync(enrollment);
        }

        public async Task DropStudentAsync(int enrollmentId, string? remarks = null)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new Exception("Enrollment not found.");

            enrollment.Status = EnrollmentStatus.Dropped;
            if (!string.IsNullOrEmpty(remarks))
                enrollment.Remarks = remarks;

            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task CompleteEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new Exception("Enrollment not found.");

            enrollment.Status = EnrollmentStatus.Completed;
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task UpdateEnrollmentAsync(Enrollment enrollment)
        {
            await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task DeleteEnrollmentAsync(int id)
        {
            await _enrollmentRepository.DeleteAsync(id);
        }
    }
}
