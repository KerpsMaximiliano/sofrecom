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
        private readonly IMenuRepository _menuRepository;
        private readonly IUserGroupRepository _userGroupRepository;
        private readonly IRoleRepository _roleRepository;

        public MenuService(IMenuRepository menuRepository, IUserGroupRepository userGroupRepository, IRoleRepository roleRepository)
        {
            _menuRepository = menuRepository;
            _userGroupRepository = userGroupRepository;
            _roleRepository = roleRepository;
        }

        public IList<Menu> GetMenu(string userName)
        {
            var groupsId = _userGroupRepository.GetGroupsId(userName);

            var roles = _roleRepository.GetRolesByGroup(groupsId);

            var roleModule = _menuRepository.GetMenuByRoles(roles.Select(x => x.Id));

            return roleModule.Select(x => x.Module.Menu).Distinct().ToList();
        }
    }
}
