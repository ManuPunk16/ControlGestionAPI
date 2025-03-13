using ControlGestionAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public interface IRoleService
    {
        Task<Role> GetRoleById(string id);
        Task<Role> GetRoleByName(string name);
        Task<List<Role>> GetAllRoles();
        Task<Role> CreateRole(Role role);
        Task<Role> UpdateRole(Role role);
        Task<bool> DeleteRole(string id);
        Task<List<Role>> GetRolesByNames(string[] roleNames);

    }
}