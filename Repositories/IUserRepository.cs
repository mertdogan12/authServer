using System.Threading.Tasks;
using authServer.Models;

namespace authServer.Repositories
{
    public interface IUserRepository
    {
        /**
         * <summary> registers an User </summary>
         *
         * <param name="user">The User whitch gets registert</param>
         */
        Task register(User user);

        /**
         * <summary>gets an User</summary>
         *
         * <param name="name">Name of the User</param>
         * <return>The User</return>
         */
        Task<User> getUser(string name);

        /**
         * <summary>Logs an User in</summary>
         *
         * <param name="name">User name</param>
         * <param name="password">Login Password</param>
         * <return>Jws Token</return>
         */
        Task<string> login(string name, string password);
    }
}
