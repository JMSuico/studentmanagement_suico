using System.Collections.Generic;
using System.Threading.Tasks;
using StudentManagement.Features.Data.Models;

namespace StudentManagement.Features.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(User user, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task UpdateUserAsync(User user);
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task AdminResetPasswordAsync(int userId, string newPassword);
        Task DeleteUserAsync(int id);
    }
}
