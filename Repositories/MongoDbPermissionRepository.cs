using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using MongoDB.Driver;
using authServer.Models;

namespace authServer.Repositories
{
    public class MongoPermissionRepository : IPermissionRepository
    {
        private const string collectionName = "Permissions";
        private readonly IMongoCollection<Permission> collection;
        private readonly FilterDefinitionBuilder<Permission> filterDefinitionBuilder = Builders<Permission>.Filter;
        private readonly IUserRepository userRepository;

        /// <summary> 
        /// initialisates the collection
        /// </summary>
        ///
        /// <param name="mongoClient">Correct MongoClient</param>
        public MongoPermissionRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(Startup.databaseName);
            collection = database.GetCollection<Permission>(collectionName);
            userRepository = new MongoDbUserRepository(mongoClient);
        }

        public async Task<string[]> getPermissions(Guid id)
        {
            var filter = filterDefinitionBuilder.Eq(permission => permission.id, id);
            Permission permissions = await collection.Find(filter).SingleOrDefaultAsync();

            return (permissions is null) ? new string[] { } : permissions.permissions;
        }

        public async Task addPermission(Guid id, string permission)
        {
            var filter = filterDefinitionBuilder.Eq(permission => permission.id, id);
            Permission permissions = await collection.Find(filter).SingleOrDefaultAsync();
            Permission newPermssions;

            if (permissions is null)
            {
                newPermssions = new()
                {
                    id = id,
                    permissions = new string[] { permission }
                };

                await collection.InsertOneAsync(newPermssions);
            }
            else
            {
                List<string> permissionsList = permissions.permissions.ToList();
                permissionsList.Add(permission);

                newPermssions = permissions with
                {
                    permissions = permissionsList.ToArray()
                };

                await collection.ReplaceOneAsync(filter, newPermssions);
            }

        }

        public async Task removePermission(Guid id, string permission)
        {
            User user = await userRepository.getUser(id);
            if (user is null) throw new Exception("User does not exist");

            var filter = filterDefinitionBuilder.Eq(permission => permission.id, user.id);
            Permission permissions = await collection.Find(filter).SingleOrDefaultAsync();

            if (permission is null) return;

            List<string> permissionsList = permissions.permissions.ToList();
            permissionsList.Remove(permission);

            Permission newPermssions = permissions with
            {
                permissions = permissionsList.ToArray()
            };

            await collection.ReplaceOneAsync(filter, newPermssions);
        }

        public async Task<bool> hasPermission(Guid id, string permissionGroup, string permission)
        {
            string[] permissions = await getPermissions(id);

            if (permission is null) return false;

            if (Array.Exists(permissions, element => element == permissionGroup + "." + permission)
                    || Array.Exists(permissions, element => element == permissionGroup + ".*")
                    || Array.Exists(permissions, element => element == "*"))
            {
                return true;
            }
            else
                return false;
        }
    }
}

