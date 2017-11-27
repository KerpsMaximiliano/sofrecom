using Sofco.Core.DAL.Admin;

namespace Sofco.DAL
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IGroupRepository GroupRepository { get; }
        IModuleRepository ModuleRepository { get; }
        IFunctionalityRepository FunctionalityRepository { get; }
        IUserGroupRepository UserGroupRepository { get; }

        void Save();
    }
}
