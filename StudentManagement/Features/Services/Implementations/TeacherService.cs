using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Data.Enums;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IUserService _userService;

        public TeacherService(ITeacherRepository teacherRepository, IUserService userService)
        {
            _teacherRepository = teacherRepository;
            _userService = userService;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _teacherRepository.GetAllAsync();
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _teacherRepository.GetByIdAsync(id);
        }

        public async Task<Teacher?> GetTeacherByEmployeeNumberAsync(string employeeNumber)
        {
            return await _teacherRepository.GetByEmployeeNumberAsync(employeeNumber);
        }

        public async Task<Teacher> CreateTeacherAsync(Teacher teacher)
        {
            var existing = await _teacherRepository.GetByEmployeeNumberAsync(teacher.EmployeeNumber);
            if (existing != null)
                throw new Exception($"Teacher with employee number {teacher.EmployeeNumber} already exists.");

            // Create linked user account
            var username = !string.IsNullOrWhiteSpace(teacher.SystemUsername) ? teacher.SystemUsername : teacher.Email;
            if (string.IsNullOrWhiteSpace(username))
            {
                username = teacher.EmployeeNumber;
            }

            var existingUser = await _userService.GetUserByUsernameAsync(username);
            if (existingUser != null)
            {
                throw new Exception($"A user with username/email '{username}' already exists. Please use a different username/email.");
            }

            var newUser = new User
            {
                Username = username,
                Email = teacher.Email ?? $"{teacher.EmployeeNumber}@school.edu",
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                PasswordHash = "", // Will be overwritten by RegisterAsync
                Role = UserRole.Teacher,
                IsActive = teacher.IsActive
            };

            var password = !string.IsNullOrWhiteSpace(teacher.SystemPassword) ? teacher.SystemPassword : teacher.EmployeeNumber;
            await _userService.RegisterAsync(newUser, password);

            teacher.CreatedAt = DateTime.UtcNow;
            return await _teacherRepository.CreateAsync(teacher);
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            var oldTeacher = await _teacherRepository.GetByIdAsync(teacher.Id);
            if (oldTeacher != null)
            {
                var usernameToFind = string.IsNullOrWhiteSpace(oldTeacher.Email) ? oldTeacher.EmployeeNumber : oldTeacher.Email;
                var user = await _userService.GetUserByUsernameAsync(usernameToFind);
                if (user != null)
                {
                    var newUsername = !string.IsNullOrWhiteSpace(teacher.SystemUsername) ? teacher.SystemUsername : (string.IsNullOrWhiteSpace(teacher.Email) ? teacher.EmployeeNumber : teacher.Email);
                    user.Username = newUsername;
                    user.Email = teacher.Email ?? $"{teacher.EmployeeNumber}@school.edu";
                    user.FirstName = teacher.FirstName;
                    user.LastName = teacher.LastName;
                    user.IsActive = teacher.IsActive;

                    try 
                    {
                        await _userService.UpdateUserAsync(user);

                        if (!string.IsNullOrWhiteSpace(teacher.SystemPassword))
                        {
                            await _userService.AdminResetPasswordAsync(user.Id, teacher.SystemPassword);
                        }
                    }
                    catch (Exception)
                    {
                        // Ignore unique constraint errors on update if admin didn't change email to a conflicting one
                    }
                }
            }

            teacher.UpdatedAt = DateTime.UtcNow;
            await _teacherRepository.UpdateAsync(teacher);
        }

        public async Task DeleteTeacherAsync(int id)
        {
            var teacher = await _teacherRepository.GetByIdAsync(id);
            if (teacher != null)
            {
                var username = string.IsNullOrWhiteSpace(teacher.Email) ? teacher.EmployeeNumber : teacher.Email;
                var user = await _userService.GetUserByUsernameAsync(username);
                if (user != null)
                {
                    await _userService.DeleteUserAsync(user.Id);
                }
            }

            await _teacherRepository.DeleteAsync(id);
        }
    }
}
