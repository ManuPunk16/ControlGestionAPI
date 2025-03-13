using ControlGestionAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ControlGestionAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> _usersCollection;
        private readonly IMongoCollection<Role> _rolesCollection;

        public UserService(IMongoDatabase database)
        {
            _usersCollection = database.GetCollection<User>("users");
            _rolesCollection = database.GetCollection<Role>("roles");
        }

        public async Task<User> GetUserById(string id)
        {
            var user = await _usersCollection.Find(user => user.Id == id).FirstOrDefaultAsync();
            if (user != null)
            {
                await PopulateUserRoles(user);
            }
            return user;
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _usersCollection.Find(user => user.Username == username).FirstOrDefaultAsync();
            if (user != null)
            {
                await PopulateUserRoles(user);

                if (user != null)
                {
                    await PopulateUserRoles(user);
                    if (user.PopulatedRoles != null)
                    {
                        foreach (var role in user.PopulatedRoles)
                        {
                            Console.WriteLine($"Role Name: {role.Name}, Role Id: {role.Id}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("User roles are null.");
                    }
                }
            }
            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            var users = await _usersCollection.Find(_ => true).ToListAsync();
            foreach (var user in users)
            {
                await PopulateUserRoles(user);
            }
            return users;
        }

        public async Task<User> CreateUser(User user)
        {
            await _usersCollection.InsertOneAsync(user);
            await PopulateUserRoles(user);
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            await _usersCollection.ReplaceOneAsync(u => u.Id == user.Id, user);
            await PopulateUserRoles(user);
            return user;
        }

        public async Task<bool> DeleteUser(string id)
        {
            var result = await _usersCollection.DeleteOneAsync(user => user.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UsernameExists(string username)
        {
            var user = await _usersCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
            return user != null;
        }

        private async Task PopulateUserRoles(User user)
        {
            if (user.Roles != null && user.Roles.Count > 0)
            {
                var allRoles = await _rolesCollection.Find(_ => true).ToListAsync();
                var roles = allRoles.Where(r => user.Roles.Contains(ObjectId.Parse(r.Id))).ToList();

                user.Roles = roles.Select(r => ObjectId.Parse(r.Id)).ToList();
                user.PopulatedRoles = roles;
            }
        }
    }
}