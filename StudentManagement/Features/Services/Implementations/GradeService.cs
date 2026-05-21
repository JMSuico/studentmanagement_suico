using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class GradeService : IGradeService
    {
        private readonly IGradeRepository _gradeRepository;
        private readonly ITaskRepository _taskRepository;

        public GradeService(IGradeRepository gradeRepository, ITaskRepository taskRepository)
        {
            _gradeRepository = gradeRepository;
            _taskRepository = taskRepository;
        }

        public async Task<IEnumerable<Grade>> GetAllGradesAsync()
        {
            return await _gradeRepository.GetAllAsync();
        }

        public async Task<Grade?> GetGradeByIdAsync(int id)
        {
            return await _gradeRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Grade>> GetStudentGradesAsync(int studentId)
        {
            return await _gradeRepository.GetByStudentIdAsync(studentId);
        }

        public async Task<IEnumerable<Grade>> GetClassSectionGradesAsync(int classSectionId)
        {
            return await _gradeRepository.GetByClassSectionIdAsync(classSectionId);
        }

        public async Task<Grade> SubmitGradeAsync(int studentId, int classSectionId, int subjectId, decimal? prelim, decimal? midterm, decimal? final, string? remarks = null)
        {
            var existing = await _gradeRepository.GetByStudentAndSubjectAsync(studentId, classSectionId, subjectId);
            
            var overall = CalculateOverallGrade(prelim, midterm, final);

            if (existing != null)
            {
                existing.PrelimGrade = prelim ?? existing.PrelimGrade;
                existing.MidtermGrade = midterm ?? existing.MidtermGrade;
                existing.FinalGrade = final ?? existing.FinalGrade;
                existing.OverallGrade = overall ?? existing.OverallGrade;
                existing.Remarks = remarks ?? existing.Remarks;
                existing.Status = GradeStatus.Submitted;
                existing.UpdatedAt = DateTime.UtcNow;

                await _gradeRepository.UpdateAsync(existing);
                return existing;
            }

            var grade = new Grade
            {
                StudentId = studentId,
                ClassSectionId = classSectionId,
                SubjectId = subjectId,
                PrelimGrade = prelim,
                MidtermGrade = midterm,
                FinalGrade = final,
                OverallGrade = overall,
                Status = GradeStatus.Submitted,
                Remarks = remarks,
                CreatedAt = DateTime.UtcNow
            };

            return await _gradeRepository.CreateAsync(grade);
        }

        public async Task UpdateGradeAsync(Grade grade)
        {
            grade.OverallGrade = CalculateOverallGrade(grade.PrelimGrade, grade.MidtermGrade, grade.FinalGrade) ?? grade.OverallGrade;
            grade.UpdatedAt = DateTime.UtcNow;
            await _gradeRepository.UpdateAsync(grade);
        }

        public async Task ApproveGradeAsync(int gradeId)
        {
            var grade = await _gradeRepository.GetByIdAsync(gradeId);
            if (grade == null) throw new Exception("Grade not found");

            grade.Status = GradeStatus.Approved;
            grade.UpdatedAt = DateTime.UtcNow;
            await _gradeRepository.UpdateAsync(grade);
        }

        public async Task ReleaseGradeAsync(int gradeId)
        {
            var grade = await _gradeRepository.GetByIdAsync(gradeId);
            if (grade == null) throw new Exception("Grade not found");

            grade.Status = GradeStatus.Released;
            grade.UpdatedAt = DateTime.UtcNow;
            await _gradeRepository.UpdateAsync(grade);
        }

        public async Task DeleteGradeAsync(int id)
        {
            await _gradeRepository.DeleteAsync(id);
        }

        private static decimal? CalculateOverallGrade(decimal? prelim, decimal? midterm, decimal? final)
        {
            if (prelim.HasValue && midterm.HasValue && final.HasValue)
            {
                // Applied the 30/30/40 weighted formula as per Approach.md
                return (prelim.Value * 0.30m) + (midterm.Value * 0.30m) + (final.Value * 0.40m);
            }
            return null;
        }

        public async Task<IEnumerable<SubjectGradeSetup>> GetAllGradeSetupsAsync()
        {
            return await _gradeRepository.GetAllGradeSetupsAsync();
        }

        public async Task<SubjectGradeSetup?> GetGradeSetupAsync(int subjectId, string semester)
        {
            return await _gradeRepository.GetGradeSetupAsync(subjectId, semester);
        }

        public async Task<SubjectGradeSetup> SaveGradeSetupAsync(SubjectGradeSetup setup)
        {
            var existing = await _gradeRepository.GetGradeSetupAsync(setup.SubjectId, setup.Semester);

            if (existing != null)
            {
                existing.QuizzesPercentage = setup.QuizzesPercentage;
                existing.LongExamPercentage = setup.LongExamPercentage;
                existing.PerformancePercentage = setup.PerformancePercentage;
                existing.SemesterExamPercentage = setup.SemesterExamPercentage;
                existing.HandsOnPercentage = setup.HandsOnPercentage;
                existing.PracticalPercentage = setup.PracticalPercentage;
                existing.OralPercentage = setup.OralPercentage;
                
                await _gradeRepository.UpdateGradeSetupAsync(existing);
                return existing;
            }
            else
            {
                return await _gradeRepository.CreateGradeSetupAsync(setup);
            }
        }

        public async Task RecalculateStudentGradeAsync(int studentId, int subjectId, string semester)
        {
            // 1. Get Grade Setup
            var setup = await GetGradeSetupAsync(subjectId, semester);
            if (setup == null) return;

            // 2. Get Student Tasks for this subject
            var taskResultsList = await _taskRepository.GetResultsForStudentAsync(studentId, subjectId);
            var taskResults = taskResultsList.ToList();

            // 3. Aggregate scores per category (using ComputedScore which is already weighted internally or just summing and then applying category weight)
            // Assuming ComputedScore is just (PointsEarned / MaxPoints) * 100 for that specific task.
            // Actually, we'll calculate the weighted average for each category.
            decimal finalGrade = 0;

            var categories = Enum.GetValues<TaskCategory>();
            foreach (var category in categories)
            {
                var categoryResults = taskResults.Where(r => r.ClassTask!.Category == category && r.PointsEarned.HasValue).ToList();
                if (categoryResults.Any())
                {
                    // Calculate total earned / total max for this category
                    decimal totalEarned = categoryResults.Sum(r => r.PointsEarned!.Value);
                    decimal totalMax = categoryResults.Sum(r => r.ClassTask!.MaxPoints);
                    
                    if (totalMax > 0)
                    {
                        decimal categoryScorePercentage = (totalEarned / totalMax) * 100m;
                        
                        // Apply weight
                        decimal weight = 0;
                        switch (category)
                        {
                            case TaskCategory.Quiz: weight = setup.QuizzesPercentage; break;
                            case TaskCategory.LongExam: weight = setup.LongExamPercentage; break;
                            case TaskCategory.Performance: weight = setup.PerformancePercentage; break;
                            case TaskCategory.SemesterExam: weight = setup.SemesterExamPercentage; break;
                        }
                        
                        finalGrade += (categoryScorePercentage * (weight / 100m));
                    }
                }
            }

            // 4. Update the Grade record
            var grades = await _gradeRepository.GetByStudentIdAsync(studentId);
            var gradeRecord = grades.FirstOrDefault(g => g.SubjectId == subjectId);

            if (gradeRecord != null)
            {
                gradeRecord.OverallGrade = finalGrade; // Automatically calculated
                await _gradeRepository.UpdateAsync(gradeRecord);
            }
        }
    }
}
