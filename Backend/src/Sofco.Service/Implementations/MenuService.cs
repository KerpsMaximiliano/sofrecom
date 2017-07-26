using Sofco.Core.Services;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Core.Interfaces.DAL;

namespace Sofco.Service.Implementations
{
    public class MenuService : IMenuService
    {
        private readonly IRoleRepository _roleRepository;

        public MenuService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public IList<Menu> GetMenuByRoleId(int[] roleIds)
        {
            return _roleRepository.GetMenusByRoleId(roleIds);
        }
    }
}
