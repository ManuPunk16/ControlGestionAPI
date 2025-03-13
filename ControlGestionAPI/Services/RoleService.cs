using ControlGestionAPI.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public class RoleService : IRoleService
    {
        private readonly IMongoCollection<Role> _rolesCollection;

        public RoleService(IMongoDatabase database)
        {
            _rolesCollection = database.GetCollection<Role>("roles");
        }

        public async Task<Role> GetRoleById(string id)
        {
            return await _rolesCollection.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Role> GetRoleByName(string name)
        {
            return await _rolesCollection.Find(role => role.Name == name).FirstOrDefaultAsync();
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _rolesCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Role> CreateRole(Role role)
        {
            await _rolesCollection.InsertOneAsync(role);
            return role;
        }

        public async Task<Role> UpdateRole(Role role)
        {
            await _rolesCollection.ReplaceOneAsync(r => r.Id == role.Id, role);
            return role;
        }

        public async Task<bool> DeleteRole(string id)
        {
            var result = await _rolesCollection.DeleteOneAsync(role => role.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<List<Role>> GetRolesByNames(string[] roleNames)
        {
            var filter = Builders<Role>.Filter.In(r => r.Name, roleNames);
            return await _rolesCollection.Find(filter).ToListAsync();
        }
    }
}