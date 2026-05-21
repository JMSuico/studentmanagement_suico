using System;
using System.Collections.Generic;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Data.Models
{
    public class ClassSection
    {
        public int Id { get; set; }
        public required string ClassName { get; set; }
        public required string SectionName { get; set; }
        public required string SchoolYear { get; set; }
        public required string Semester { get; set; }
        
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        
        public int TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        
        public required string Schedule { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? DaysOfWeek { get; set; } // e.g., "MWF", "TTh"
        
        public required string Room { get; set; }
        public int MaxCapacity { get; set; }
        public ClassStatus Status { get; set; } = ClassStatus.Active;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<ClassSchedule> Schedules { get; set; } = new List<ClassSchedule>();
    }
}
