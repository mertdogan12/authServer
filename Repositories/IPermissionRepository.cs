using System.Threading.Tasks;
using authServer.Models;

namespace authServer.Repositories
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// Gets all permission from an User
        /// </summary>
        /// <param name="user"/>
        public Task<string[]> getPermissions(User user);

        /// <summary>
        /// Adds an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task addPermission(User user, string permission);

        /// <summary>
        /// Removes an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task removePermission(User user, string permission);

        /// <summary>
        /// Checks if the user has an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task<bool> hasPermission(User user, string permission);
    }
}
