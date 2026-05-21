using System;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        public int ClassSectionId { get; set; }
        public ClassSection? ClassSection { get; set; }
        
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;
        public string? Remarks { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
