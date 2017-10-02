using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations.Admin
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IRoleRepository _roleRepository;

        public MenuService(IMenuRepository menuRepository, IUserGroupRepository userGroupRepository, IRoleRepository roleRepository)
        {
            _menuRepository = menuRepository;
            _userGroupRepository = userGroupRepository;
            _roleRepository = roleRepository;
        }

        public IList<RoleFunctionality> GetFunctionalitiesByUserName(string userName)
        {
            var groupsId = _userGroupRepository.GetGroupsId(userName);

            var roles = _roleRepository.GetRolesByGroup(groupsId);

            var modules = _menuRepository.GetFunctionalitiesByRoles(roles.Select(x => x.Id));

            return modules;
        }
    }
}
