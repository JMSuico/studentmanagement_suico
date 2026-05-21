using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagement.Features.Data.Models
{
    public class Teacher
    {
        public int Id { get; set; }
        public required string EmployeeNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string ContactNumber { get; set; }
        public required string Department { get; set; }
        public string? Specialization { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ProfileImage { get; set; }
        
        [NotMapped]
        public string? SystemUsername { get; set; }
        [NotMapped]
        public string? SystemPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
    }
}
