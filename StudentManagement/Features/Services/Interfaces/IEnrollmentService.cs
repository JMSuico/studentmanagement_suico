using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync();
        Task<Enrollment?> GetEnrollmentByIdAsync(int id);
        Task<IEnumerable<Enrollment>> GetStudentEnrollmentsAsync(int studentId);
        Task<IEnumerable<Enrollment>> GetClassSectionEnrollmentsAsync(int classSectionId);
        Task<Enrollment> EnrollStudentAsync(int studentId, int classSectionId, string? remarks = null, EnrollmentStatus status = EnrollmentStatus.Enrolled);
        Task DropStudentAsync(int enrollmentId, string? remarks = null);
        Task CompleteEnrollmentAsync(int enrollmentId);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task DeleteEnrollmentAsync(int id);
    }
}
