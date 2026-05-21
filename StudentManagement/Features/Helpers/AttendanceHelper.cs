using System.Collections.Generic;
using System.Linq;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;

namespace StudentManagement.Features.Helpers
{
    public static class AttendanceHelper
    {
        public static double CalculateAttendancePercentage(IEnumerable<Attendance> attendances)
        {
            var list = attendances.ToList();
            if (!list.Any()) return 0;

            var presentCount = list.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
            return (double)presentCount / list.Count * 100;
        }

        public static string GetAttendanceSummary(IEnumerable<Attendance> attendances)
        {
            var list = attendances.ToList();
            var present = list.Count(a => a.Status == AttendanceStatus.Present);
            var late = list.Count(a => a.Status == AttendanceStatus.Late);
            var absent = list.Count(a => a.Status == AttendanceStatus.Absent);
            var excused = list.Count(a => a.Status == AttendanceStatus.Excused);

            return $"P:{present} L:{late} A:{absent} E:{excused}";
        }
    }
}
