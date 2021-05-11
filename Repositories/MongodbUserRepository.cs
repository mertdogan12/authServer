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
    }
}
