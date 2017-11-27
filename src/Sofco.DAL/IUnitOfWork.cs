using Sofco.Core.DAL.Admin;

namespace Sofco.DAL
{
    public interface IUnitOfWork
    {
        #region Admin

        IUserRepository UserRepository { get; }
        IRoleRepository RoleRepository { get; }
        IGroupRepository GroupRepository { get; }
        IModuleRepository ModuleRepository { get; }
        IFunctionalityRepository FunctionalityRepository { get; }
        IUserGroupRepository UserGroupRepository { get; }
        IMenuRepository MenuRepository { get; }
        IGlobalSettingRepository GlobalSettingRepository { get; }
        IRoleFunctionalityRepository RoleFunctionalityRepository { get; }

        #endregion

        void Save();
    }
}
