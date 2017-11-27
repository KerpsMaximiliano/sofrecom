using Sofco.Core.DAL.Admin;
using Sofco.DAL.Repositories.Admin;

namespace Sofco.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SofcoContext context;

        private IUserRepository userRepository;
        private IRoleRepository roleRepository;
        private IGroupRepository groupRepository;
        private IModuleRepository moduleRepository;
        private IFunctionalityRepository functionalityRepository;
        private IUserGroupRepository userGroupRepository;

        public UnitOfWork(SofcoContext context)
        {
            this.context = context;
        }

        public IUserRepository UserRepository => userRepository ?? (userRepository = new UserRepository(context));

        public IRoleRepository RoleRepository => roleRepository ?? (roleRepository = new RoleRepository(context));

        public IGroupRepository GroupRepository => groupRepository ?? (groupRepository = new GroupRepository(context));

        public IModuleRepository ModuleRepository => moduleRepository ?? (moduleRepository = new ModuleRepository(context));

        public IFunctionalityRepository FunctionalityRepository => functionalityRepository ?? (functionalityRepository = new FunctionalityRepository(context));

        public IUserGroupRepository UserGroupRepository => userGroupRepository ?? (userGroupRepository = new UserGroupRepository(context));

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
