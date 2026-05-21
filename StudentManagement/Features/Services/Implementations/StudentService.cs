using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IUserService _userService;

        public StudentService(IStudentRepository studentRepository, IUserService userService)
        {
            _studentRepository = studentRepository;
            _userService = userService;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _studentRepository.GetAllAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _studentRepository.GetByIdAsync(id);
        }

        public async Task<Student?> GetStudentByNumberAsync(string studentNumber)
        {
            return await _studentRepository.GetByStudentNumberAsync(studentNumber);
        }

        public async Task<IEnumerable<Student>> SearchStudentsAsync(string searchTerm)
        {
            return await _studentRepository.SearchAsync(searchTerm);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            var existing = await _studentRepository.GetByStudentNumberAsync(student.StudentNumber);
            if (existing != null)
                throw new Exception($"Student with number {student.StudentNumber} already exists.");

            // Create linked user account
            var username = !string.IsNullOrWhiteSpace(student.SystemUsername) ? student.SystemUsername : student.Email;
            if (string.IsNullOrWhiteSpace(username))
            {
                username = student.StudentNumber;
            }

            var existingUser = await _userService.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new Exception($"A user with username/email '{username}' already exists. Please use a different username/email.");
            }

            var newUser = new User
            {
                Username = username,
                Email = student.Email ?? $"{student.StudentNumber}@school.edu",
                FirstName = student.FirstName,
                LastName = student.LastName,
                PasswordHash = "", // Will be overwritten by RegisterAsync
                Role = UserRole.Student,
                IsActive = true
            };

            var password = !string.IsNullOrWhiteSpace(student.SystemPassword) ? student.SystemPassword : student.StudentNumber;
            await _userService.RegisterAsync(newUser, password);

            student.CreatedAt = DateTime.UtcNow;
            return await _studentRepository.CreateAsync(student);
        }

        public async Task UpdateStudentAsync(Student student)
        {
            var oldStudent = await _studentRepository.GetByIdAsync(student.Id);
            if (oldStudent != null)
            {
                var oldUsername = string.IsNullOrWhiteSpace(oldStudent.SystemUsername) 
                    ? (string.IsNullOrWhiteSpace(oldStudent.Email) ? oldStudent.StudentNumber : oldStudent.Email) 
                    : oldStudent.SystemUsername;

                // Fallback lookup if oldUsername is null, let's just find by original known username.
                // Wait, SystemUsername is not saved in DB for Student, so oldStudent.SystemUsername will be null.
                var usernameToFind = string.IsNullOrWhiteSpace(oldStudent.Email) ? oldStudent.StudentNumber : oldStudent.Email;
                
                var user = await _userService.GetUserByUsernameAsync(usernameToFind);
                if (user != null)
                {
                    var newUsername = !string.IsNullOrWhiteSpace(student.SystemUsername) ? student.SystemUsername : (string.IsNullOrWhiteSpace(student.Email) ? student.StudentNumber : student.Email);
                    user.Username = newUsername;
                    user.Email = student.Email ?? $"{student.StudentNumber}@school.edu";
                    user.FirstName = student.FirstName;
                    user.LastName = student.LastName;
                    
                    try 
                    {
                        await _userService.UpdateUserAsync(user);

                        if (!string.IsNullOrWhiteSpace(student.SystemPassword))
                        {
                            await _userService.AdminResetPasswordAsync(user.Id, student.SystemPassword);
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore unique constraint errors on update if admin didn't change email to a conflicting one
                    }
                }
            }

            student.UpdatedAt = DateTime.UtcNow;
            await _studentRepository.UpdateAsync(student);
        }

        public async Task PromoteAllStudentsAsync()
        {
            var allStudents = await _studentRepository.GetAllAsync();
            foreach (var student in allStudents)
            {
                if (student.Status == StudentStatus.Active)
                {
                    if (student.YearLevel >= 4)
                    {
                        student.Status = StudentStatus.Graduated;
                    }
                    else
                    {
                        student.YearLevel += 1;
                    }
                    student.UpdatedAt = DateTime.UtcNow;
                    await _studentRepository.UpdateAsync(student);
                }
            }
        }

        public async Task RevertPromoteAllStudentsAsync()
        {
            var allStudents = await _studentRepository.GetAllAsync();
            foreach (var student in allStudents)
            {
                if (student.Status == StudentStatus.Graduated)
                {
                    student.Status = StudentStatus.Active;
                    student.YearLevel = 4;
                    student.UpdatedAt = DateTime.UtcNow;
                    await _studentRepository.UpdateAsync(student);
                }
                else if (student.Status == StudentStatus.Active && student.YearLevel > 1)
                {
                    student.YearLevel -= 1;
                    student.UpdatedAt = DateTime.UtcNow;
                    await _studentRepository.UpdateAsync(student);
                }
            }
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student != null)
            {
                var username = string.IsNullOrWhiteSpace(student.Email) ? student.StudentNumber : student.Email;
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    await _userService.DeleteUserAsync(user.Id);
                }
            }

            await _studentRepository.DeleteAsync(id);
        }
    }
}
