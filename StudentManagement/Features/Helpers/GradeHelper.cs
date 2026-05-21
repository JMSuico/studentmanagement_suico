namespace StudentManagement.Features.Helpers
{
    public static class GradeHelper
    {
        public static string GetGradeRemarks(decimal? grade)
        {
            if (!grade.HasValue) return "Incomplete";

            if (grade.Value >= 75) return "Passed";
            return "Failed";
        }

        public static string GetGradeEquivalent(decimal? grade)
        {
            if (!grade.HasValue) return "INC";

            // Assuming a 1.0 - 5.0 grading system where 1.0 is excellent and 3.0 is passing.
            // This can be adjusted based on the specific institution's policy.
            if (grade >= 98) return "1.00";
            if (grade >= 95) return "1.25";
            if (grade >= 92) return "1.50";
            if (grade >= 89) return "1.75";
            if (grade >= 86) return "2.00";
            if (grade >= 83) return "2.25";
            if (grade >= 80) return "2.50";
            if (grade >= 77) return "2.75";
            if (grade >= 75) return "3.00";
            
            return "5.00"; // Failed
        }
    }
}
