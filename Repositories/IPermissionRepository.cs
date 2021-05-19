using System.Threading.Tasks;

namespace authServer.Repositories
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// Gets all permission from an User
        /// </summary>
        /// <param name="id"/>
        public Task<string[]> getPermissions(string name);

        /// <summary>
        /// Adds an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task addPermission(string name, string permission);

        /// <summary>
        /// Removes an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task removePermission(string name, string permission);

        /// <summary>
        /// Checks if the user has an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task<bool> hasPermission(string name, string permissionGroup, string permission);
    }
}
