using System;
using System.Collections.Generic;

namespace StudentManagement.Features.Data.Models
{
    public class Subject
    {
        public int Id { get; set; }
        public required string SubjectCode { get; set; }
        public required string SubjectName { get; set; }
        public string? Description { get; set; }
        public int Units { get; set; }
        public bool IsActive { get; set; } = true;
        public int YearLevel { get; set; } = 1;
        public string Semester { get; set; } = "1st Semester";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}
