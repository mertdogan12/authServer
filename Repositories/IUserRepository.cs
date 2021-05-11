using System.Threading.Tasks;
using authServer.Models;

namespace authServer.Repositories
{
    public interface IUserRepository
    {
        Task register(User user);
        Task<User> getUser(string name);
    }
}
