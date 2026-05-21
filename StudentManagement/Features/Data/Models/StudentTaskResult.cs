using System;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class StudentTaskResult
    {
        public int Id { get; set; }
        
        public int ClassTaskId { get; set; }
        public ClassTask? ClassTask { get; set; }
        
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        public decimal? PointsEarned { get; set; }
        public decimal? ComputedScore { get; set; } // PointsEarned / MaxPoints * category weight
        
        public ClassTaskStatus Status { get; set; } = ClassTaskStatus.NoSubmission;
        public string? Remarks { get; set; }
        
        public DateTime? SubmittedAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
