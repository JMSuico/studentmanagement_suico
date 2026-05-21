using Microsoft.EntityFrameworkCore;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<ClassSection> ClassSections { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<Attendance> Attendances { get; set; } = null!;
        public DbSet<Grade> Grades { get; set; } = null!;
        public DbSet<Guardian> Guardians { get; set; } = null!;
        
        public DbSet<AttendanceSession> AttendanceSessions { get; set; } = null!;
        public DbSet<SubjectGradeSetup> SubjectGradeSetups { get; set; } = null!;
        public DbSet<ClassTask> ClassTasks { get; set; } = null!;
        public DbSet<StudentTaskResult> StudentTaskResults { get; set; } = null!;
        public DbSet<ClassSchedule> ClassSchedules { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Student configuration
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Guardian)
                .WithMany(g => g.Students)
                .HasForeignKey(s => s.GuardianId)
                .OnDelete(DeleteBehavior.SetNull);

            // Teacher configuration
            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeNumber)
                .IsUnique();

            // Subject configuration
            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.SubjectCode)
                .IsUnique();

            // ClassSection configuration
            modelBuilder.Entity<ClassSection>()
                .HasOne(c => c.Teacher)
                .WithMany(t => t.ClassSections)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ClassSection>()
                .HasOne(c => c.Subject)
                .WithMany(s => s.ClassSections)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment configuration
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.ClassSection)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.ClassSectionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.ClassSectionId })
                .IsUnique();

            // Attendance configuration
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.ClassSection)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassSectionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Attendance>()
                .HasIndex(a => new { a.StudentId, a.ClassSectionId, a.Date })
                .IsUnique();

            // Grade configuration
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Subject)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.ClassSection)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.ClassSectionId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Grade>()
                .HasIndex(g => new { g.StudentId, g.ClassSectionId, g.SubjectId })
                .IsUnique();

            // ClassSchedule configuration
            modelBuilder.Entity<ClassSchedule>()
                .HasOne(cs => cs.ClassSection)
                .WithMany(c => c.Schedules)
                .HasForeignKey(cs => cs.ClassSectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // AttendanceSession configuration
            modelBuilder.Entity<AttendanceSession>()
                .HasOne(a => a.ClassSection)
                .WithMany()
                .HasForeignKey(a => a.ClassSectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance to AttendanceSession configuration
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.AttendanceSession)
                .WithMany()
                .HasForeignKey(a => a.AttendanceSessionId)
                .OnDelete(DeleteBehavior.SetNull);

            // SubjectGradeSetup configuration
            modelBuilder.Entity<SubjectGradeSetup>()
                .HasOne(s => s.Subject)
                .WithMany()
                .HasForeignKey(s => s.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<SubjectGradeSetup>()
                .HasIndex(s => new { s.SubjectId, s.Semester })
                .IsUnique();

            // ClassTask configuration
            modelBuilder.Entity<ClassTask>()
                .HasOne(t => t.Subject)
                .WithMany()
                .HasForeignKey(t => t.SubjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // StudentTaskResult configuration
            modelBuilder.Entity<StudentTaskResult>()
                .HasOne(r => r.ClassTask)
                .WithMany()
                .HasForeignKey(r => r.ClassTaskId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StudentTaskResult>()
                .HasOne(r => r.Student)
                .WithMany()
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<StudentTaskResult>()
                .HasIndex(r => new { r.ClassTaskId, r.StudentId })
                .IsUnique();
        }
    }
}
