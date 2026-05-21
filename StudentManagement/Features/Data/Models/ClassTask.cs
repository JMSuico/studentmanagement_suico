using System;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class ClassTask
    {
        public int Id { get; set; }
        
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        
        public TaskCategory Category { get; set; }
        
        public required string Title { get; set; }
        public string? Description { get; set; }
        
        public decimal MaxPoints { get; set; }
        public DateTime Deadline { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
