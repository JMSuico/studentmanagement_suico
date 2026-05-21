using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class Student
    {
        public int Id { get; set; }
        public required string StudentNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? MiddleName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required string Address { get; set; }
        public required string ContactNumber { get; set; }
        public string? Email { get; set; }
        public StudentStatus Status { get; set; } = StudentStatus.Active;
        public int YearLevel { get; set; }
        public string? Section { get; set; }
        public string? ProfileImage { get; set; }
        
        [NotMapped]
        public string? SystemUsername { get; set; }
        [NotMapped]
        public string? SystemPassword { get; set; }

        public int? GuardianId { get; set; }
        public Guardian? Guardian { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
