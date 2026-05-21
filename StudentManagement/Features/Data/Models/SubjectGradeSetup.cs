namespace StudentManagement.Features.Data.Models
{
    public class SubjectGradeSetup
    {
        public int Id { get; set; }
        
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        
        public required string Semester { get; set; }
        
        // Main categories (must sum to 100)
        public decimal QuizzesPercentage { get; set; } = 10;
        public decimal LongExamPercentage { get; set; } = 15;
        public decimal PerformancePercentage { get; set; } = 60;
        public decimal SemesterExamPercentage { get; set; } = 15;
        
        // Performance breakdown (must sum to PerformancePercentage, or be percentage of Performance)
        // Let's assume these are percentages OF the 60%. e.g., HandsOn is 20% OF the total grade (so 1/3 of Performance).
        // The spec says: Performance (60%) -> Hands-on 20%, Practical 20%, Oral 20% (These sum to 60%)
        public decimal HandsOnPercentage { get; set; } = 20;
        public decimal PracticalPercentage { get; set; } = 20;
        public decimal OralPercentage { get; set; } = 20;
    }
}
