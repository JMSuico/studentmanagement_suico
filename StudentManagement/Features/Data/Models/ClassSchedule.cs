using System;

namespace StudentManagement.Features.Data.Models
{
    public class ClassSchedule
    {
        public int Id { get; set; }

        public int ClassSectionId { get; set; }
        public ClassSection? ClassSection { get; set; }

        public DateTime ScheduleDate { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsWeeklyRepeat { get; set; }
    }
}
