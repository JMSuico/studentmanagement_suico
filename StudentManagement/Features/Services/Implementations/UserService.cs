using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;
using StudentManagement.Features.Repositories.Interfaces;
using StudentManagement.Features.Services.Interfaces;

namespace StudentManagement.Features.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !user.IsActive)
                return null;

            if (VerifyPasswordHash(password, user.PasswordHash))
                return user;

            return null;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(user.Username);
            if (existingUser != null)
                throw new Exception("Username is already taken");

            var existingEmail = await _userRepository.GetByEmailAsync(user.Email);
            if (existingEmail != null)
                throw new Exception("Email is already registered");

            user.PasswordHash = CreatePasswordHash(password);
            user.CreatedAt = DateTime.UtcNow;
            
            return await _userRepository.CreateAsync(user);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            if (!VerifyPasswordHash(currentPassword, user.PasswordHash))
                throw new Exception("Incorrect current password");

            user.PasswordHash = CreatePasswordHash(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        public async Task AdminResetPasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.PasswordHash = CreatePasswordHash(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }

        private static string CreatePasswordHash(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPasswordHash(string password, string storedHash)
        {
            var hashOfInput = CreatePasswordHash(password);
            return hashOfInput == storedHash;
        }
    }
}
