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

        /// <summary> 
        /// initialisates the collection
        /// </summary>
        ///
        /// <param name="mongoClient">Correct MongoClient</param>
        public MongoPermissionRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(Startup.databaseName);
            collection = database.GetCollection<Permission>(collectionName);
        }

        public async Task<string[]> getPermissions(User user)
        {
            var filter = filterDefinitionBuilder.Eq(permission => permission.name, user.name);
            Permission permissions = await collection.Find(filter).SingleOrDefaultAsync();

            return (permissions is null) ? new string[] { } : permissions.permissions;
        }

        public async Task addPermission(User user, string permission)
        {
            var filter = filterDefinitionBuilder.Eq(permission => permission.name, user.name);
            Permission permissions = await collection.Find(filter).SingleOrDefaultAsync();
            Permission newPermssions;

            if (permissions is null)
            {
                newPermssions = new()
                {
                    id = user.id,
                    name = user.name,
                    permissions = new string[] { permission }
                };
            }
            else
            {
                List<string> permissionsList = permissions.permissions.ToList();
                permissionsList.Add(permission);

                newPermssions = permissions with
                {
                    permissions = permissionsList.ToArray()
                };
            }

            await collection.InsertOneAsync(newPermssions);
        }

        public async Task removePermission(User user, string permission)
        {
            var filter = filterDefinitionBuilder.Eq(permission => permission.name, user.name);
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

        public async Task<bool> hasPermission(User user, string permission)
        {
            string[] permissions = await getPermissions(user);

            if (permission is null) return false;

            if (Array.Exists(permissions, element => element == permission))
            {
                return true;
            }
            else
                return false;
        }
    }
}

