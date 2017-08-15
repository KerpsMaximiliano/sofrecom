using Sofco.Core.Services;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Core.DAL;
using Sofco.Core.Interfaces.DAL;
using System.Linq;

namespace Sofco.Service.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IRoleModuleFunctionalityRepository _roleModuleFunctionalityRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IRoleRepository _roleRepository;

        public MenuService(IRoleModuleFunctionalityRepository roleModuleFunctionalityRepository, IUserGroupRepository userGroupRepository, IRoleRepository roleRepository)
        {
            _roleModuleFunctionalityRepository = roleModuleFunctionalityRepository;
            _userGroupRepository = userGroupRepository;
            _roleRepository = roleRepository;
        }

        public IList<Menu> GetMenu(int userId)
        {
            var groupsId = _userGroupRepository.GetGroupsId(userId);

            var roles = _roleRepository.GetRolesByGroup(groupsId);

            var roleModuleFunctionality = _roleModuleFunctionalityRepository.GetModulesByRoles(roles.Select(x => x.Id));

            return roleModuleFunctionality.Select(x => x.Module.Menu).Distinct().ToList();
        }
    }
}
