using System;

namespace StudentManagement.Features.Helpers
{
    public static class DateHelper
    {
        public static string FormatDate(DateTime date)
        {
            return date.ToString("MMM dd, yyyy");
        }

        public static string FormatDateTime(DateTime date)
        {
            return date.ToString("MMM dd, yyyy hh:mm tt");
        }

        public static string GetSchoolYear(DateTime date)
        {
            int year = date.Year;
            if (date.Month >= 6) // Starting June
            {
                return $"{year}-{year + 1}";
            }
            return $"{year - 1}-{year}";
        }

        public static string GetSemester(DateTime date)
        {
            if (date.Month >= 8 && date.Month <= 12)
                return "1st Semester";
            if (date.Month >= 1 && date.Month <= 5)
                return "2nd Semester";
            return "Summer";
        }
    }
}
