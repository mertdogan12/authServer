using System.Threading.Tasks;
using authServer.Models;
using MongoDB.Driver;

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

            return "OK";
        }
    }
}
