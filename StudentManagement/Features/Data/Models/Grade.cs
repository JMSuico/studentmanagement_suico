using System;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class Grade
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        
        public int ClassSectionId { get; set; }
        public ClassSection? ClassSection { get; set; }
        
        public decimal? PrelimGrade { get; set; }
        public decimal? MidtermGrade { get; set; }
        public decimal? FinalGrade { get; set; }
        public decimal? OverallGrade { get; set; }
        
        public GradeStatus Status { get; set; } = GradeStatus.Pending;
        public string? Remarks { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
