using System.Threading.Tasks;
using authServer.Models;
using System;

namespace authServer.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// registers an User
        /// </summary>
        ///
        /// <param name="user">The User whitch gets registert</param>
        Task register(User user);

        /// <summary>
        /// gets an User
        /// </summary>
        ///
        /// <param name="name"/>
        /// <returns>The User</returns>
        Task<User> getUser(string name);

        /// <summary>
        /// gets an User
        /// </summary>
        ///
        /// <param name="id"/>
        /// <returns>The User</returns>
        Task<User> getUser(Guid id);

        /// <summary>
        /// Logs an User in
        /// </summary>
        ///
        /// <param name="name">User name</param>
        /// <param name="password">Login Password</param>
        /// <returns>Jws Token</returns>
        Task<string> login(string name, string password);

        /// <summary>
        /// Change the Password form an user
        /// </summary>
        ///
        /// <param name="oldPassword"/>
        /// <param name="newPassword"/>
        /// <param name="username"/>
        /// <returns/>
        Task<string> changePassword(string oldPassword, string newPassword, Guid id);

        /// <summary>
        /// Change the Username form an user
        /// </summary>
        ///
        /// <param name="oldUsername"/>
        /// <param name="newUsername"/>
        /// <returns/>
        Task<string> changeUsername(Guid id, string newUsername);

        /// <summary>
        /// Gets all users
        /// </summary/>
        /// 
        /// <returns/>
        Task<User[]> getUsers();
    }
}
