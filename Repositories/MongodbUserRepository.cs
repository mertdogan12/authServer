using System.Threading.Tasks;
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

        /**
         * <summary> 
         * initialisates the collection
         * </summary>
         *
         * <param name="mongoClient">Correct MongoClient</param>
         */
        public MongoDbUserRepository(IMongoClient mongoClient)
        {
            IMongoDatabase database = mongoClient.GetDatabase(Startup.databaseName);
            collection = database.GetCollection<User>(collectionName);
        }

        public async Task register(User user)
        {
            if (!(await getUser(user.name) is null)) return;

            await collection.InsertOneAsync(user);
        }

        public async Task<User> getUser(string name)
        {
            var filter = filterDefinitionBuilder.Eq(user => user.name, name);
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
                    new Claim(ClaimTypes.Name, name)
                }
            };
            IAuthService authService = new JWTService(model.secredKey);

            string token = authService.generateToken(model);

            if (!authService.isTokenValid(token))
                return "Token is not valid";

            return token;
        }

        public async Task<string> changePassword(string oldPassword, string newPassword, string username)
        {
            User user = await getUser(username);

            if (user is null) return "User not found";
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.hash)) return "Password is wrong";

            User newUser = user with
            {
                hash = BCrypt.Net.BCrypt.HashPassword(newPassword)
            };
            var filter = filterDefinitionBuilder.Eq(user => user.name, username);

            await collection.ReplaceOneAsync(filter, newUser);

            return "Ok";
        }
    }
}
