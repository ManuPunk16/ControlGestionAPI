using ControlGestionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public interface IUserService
    {
        Task<User> GetUserById(string id);
        Task<User> GetUserByUsername(string username);
        Task<List<User>> GetAllUsers();
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(string id);
        Task<bool> UsernameExists(string username);
    }
}