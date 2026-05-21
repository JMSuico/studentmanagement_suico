using System;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        public int ClassSectionId { get; set; }
        public ClassSection? ClassSection { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Today;
        public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
        
        public TimeSpan? TimeIn { get; set; }
        public TimeSpan? TimeOut { get; set; }
        
        public int? AttendanceSessionId { get; set; }
        public AttendanceSession? AttendanceSession { get; set; }
        
        public string? CodeUsed { get; set; }
        public DateTime? TimeSubmitted { get; set; }
        
        public string? Remarks { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
