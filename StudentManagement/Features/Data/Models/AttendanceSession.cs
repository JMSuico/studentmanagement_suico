using System;

namespace StudentManagement.Features.Data.Models
{
    public class AttendanceSession
    {
        public int Id { get; set; }
        
        public int ClassSectionId { get; set; }
        public ClassSection? ClassSection { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Today;
        
        public required string GeneratedCode { get; set; } // 6-digit code
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
