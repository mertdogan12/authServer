using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using authServer.Models;
using MongoDB.Driver;
using System.Security.Claims;
using authServer.Managers;

namespace authServer.Repositories
{
    public class MongoDbUserRepository : IUserRepository
    {
        private const string collectionName = "Users";
        private readonly IMongoCollection<User> collection;
        private readonly FilterDefinitionBuilder<User> filterDefinitionBuilder = Builders<User>.Filter;
        private readonly IAuthContainerModel model;
        private readonly IAuthService service;

        /// <summary> 
        /// initialisates the collection and the jwt service
        /// </summary>
        ///
        /// <param name="mongoClient">Correct MongoClient</param>
        public MongoDbUserRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(Startup.databaseName);
            collection = database.GetCollection<User>(collectionName);
            model = new JWTContainerModel();
            service = new JWTService(model.secredKey);
        }

        public async Task register(User user)
        {
            await collection.InsertOneAsync(user);
        }

        public async Task<User> getUser(string name)
        {
            var filter = filterDefinitionBuilder.Eq(user => user.name, name);
            var user = await collection.Find(filter).SingleOrDefaultAsync();

            return user;
        }

        public async Task<User> getUser(Guid id)
        {
            var filter = filterDefinitionBuilder.Eq(user => user.id, id);
            var user = await collection.Find(filter).SingleOrDefaultAsync();

            return user;
        }

        public async Task<string> login(string name, string password)
        {
            User user = await getUser(name);

            if (user is null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password, user.hash)) return null;

            IAuthContainerModel model = new JWTContainerModel()
            {
                claims = new Claim[] {
                    new Claim(ClaimTypes.Name, name),
                    new Claim("id", user.id.ToString())
                }
            };

            string token = service.generateToken(model);

            if (!service.isTokenValid(token))
                return "Token is not valid";

            return token;
        }

        public async Task<string> changePassword(string oldPassword, string newPassword, Guid id)
        {
            User user = await getUser(id);

            if (user is null) return "User not found";
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.hash)) return "Password is wrong";

            User newUser = user with
            {
                hash = BCrypt.Net.BCrypt.HashPassword(newPassword)
            };
            var filter = filterDefinitionBuilder.Eq(user => user.id, id);

            await collection.ReplaceOneAsync(filter, newUser);

            return "Ok";
        }

        public async Task<string> changeUsername(Guid id, string newUsername)
        {
            User user = await getUser(id);
            User user2 = await getUser(newUsername);

            if (user is null) return "User not found";
            if (!(user2 is null)) return "Username is already taken";

            User newUser = user with
            {
                name = newUsername
            };
            var filter = filterDefinitionBuilder.Eq(user => user.id, id);

            await collection.ReplaceOneAsync(filter, newUser);

            return "Ok";
        }

        public async Task<List<User>> getUsers()
        {
            List<User> userList = await collection.Find(_ => true).ToListAsync();

            return userList;
        }
    }
}
