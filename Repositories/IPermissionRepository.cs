using System.Threading.Tasks;
using System;

namespace authServer.Repositories
{
    public interface IPermissionRepository
    {
        /// <summary>
        /// Gets all permission from an User
        /// </summary>
        /// <param name="id"/>
        public Task<string[]> getPermissions(Guid id);

        /// <summary>
        /// Adds an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task addPermission(Guid id, string permission);

        /// <summary>
        /// Removes an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task removePermission(Guid id, string permission);

        /// <summary>
        /// Checks if the user has an permission
        /// </summary>
        /// <param name="user"/>
        /// <param name="permission"/>
        public Task<bool> hasPermission(Guid id, string permissionGroup, string permission);
    }
}
