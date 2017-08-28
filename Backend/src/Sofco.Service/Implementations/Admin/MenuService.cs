using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Core.DAL;
using System.Linq;
using Sofco.Core.DAL.Admin;
using Sofco.Core.Services.Admin;
using Sofco.Model.Models.Admin;

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

        public IList<Menu> GetMenu(string userName)
        {
            var groupsId = _userGroupRepository.GetGroupsId(userName);

            var roles = _roleRepository.GetRolesByGroup(groupsId);

            var roleModule = _menuRepository.GetMenuByRoles(roles.Select(x => x.Id));

            return roleModule.Select(x => x.Module.Menu).Distinct().ToList();
        }
    }
}
