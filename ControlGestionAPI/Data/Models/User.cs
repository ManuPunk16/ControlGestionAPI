using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace ControlGestionAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("username")]
        public string Username { get; set; }
        [BsonElement("email")]
        public string Email { get; set; }
        [BsonElement("area")]
        public string Area { get; set; }
        [BsonElement("password")]
        public string Password { get; set; }
        [BsonElement("roles")]
        public List<ObjectId> Roles { get; set; }
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
        [BsonElement("__v")]
        public int? V { get; set; }
        public List<Role> PopulatedRoles { get; set; }
    }

    public class Role
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("admin")]
        public bool Admin { get; set; }
        [BsonElement("__v")]
        public int? V { get; set; }
    }
}