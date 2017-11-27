using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Admin;

namespace Sofco.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SofcoContext context;

        #region Admin

        private IUserRepository userRepository;
        private IRoleRepository roleRepository;
        private IGroupRepository groupRepository;
        private IModuleRepository moduleRepository;
        private IFunctionalityRepository functionalityRepository;
        private IUserGroupRepository userGroupRepository;
        private IMenuRepository menuRepository;
        private IGlobalSettingRepository globalSettingRepository;
        private IRoleFunctionalityRepository roleFunctionalityRepository;

        #endregion

        public UnitOfWork(SofcoContext context)
        {
            this.context = context;
        }

        #region Admin
        public IUserRepository UserRepository => userRepository ?? (userRepository = new UserRepository(context));
        public IRoleRepository RoleRepository => roleRepository ?? (roleRepository = new RoleRepository(context));
        public IGroupRepository GroupRepository => groupRepository ?? (groupRepository = new GroupRepository(context));
        public IModuleRepository ModuleRepository => moduleRepository ?? (moduleRepository = new ModuleRepository(context));
        public IFunctionalityRepository FunctionalityRepository => functionalityRepository ?? (functionalityRepository = new FunctionalityRepository(context));
        public IUserGroupRepository UserGroupRepository => userGroupRepository ?? (userGroupRepository = new UserGroupRepository(context));
        public IMenuRepository MenuRepository => menuRepository ?? (menuRepository = new MenuRepository(context));
        public IGlobalSettingRepository GlobalSettingRepository => globalSettingRepository ?? (globalSettingRepository = new GlobalSettingRepository(context));
        public IRoleFunctionalityRepository RoleFunctionalityRepository => roleFunctionalityRepository ?? (roleFunctionalityRepository = new RoleFunctionalityRepository(context));

        #endregion

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
