using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations.Admin
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository menuRepository;

        private readonly IUserGroupRepository userGroupRepository;

        private readonly IRoleRepository roleRepository;

        public MenuService(IMenuRepository menuRepository, IUserGroupRepository userGroupRepository, IRoleRepository roleRepository)
        {
            this.menuRepository = menuRepository;
            this.userGroupRepository = userGroupRepository;
            this.roleRepository = roleRepository;
        }

        public IList<RoleFunctionality> GetFunctionalitiesByUserName(string userName)
        {
            var groupsId = userGroupRepository.GetGroupsId(userName);

            var roles = roleRepository.GetRolesByGroup(groupsId);

            var modules = menuRepository.GetFunctionalitiesByRoles(roles.Select(x => x.Id));

            return modules;
        }
    }
}
