using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Services.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Service.Implementations.Admin
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork unitOfWork;

        public MenuService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public IList<RoleFunctionality> GetFunctionalitiesByUserName(string userName)
        {
            var groupsId = unitOfWork.UserGroupRepository.GetGroupsId(userName);

            var roles = unitOfWork.RoleRepository.GetRolesByGroup(groupsId);

            var modules = unitOfWork.MenuRepository.GetFunctionalitiesByRoles(roles.Select(x => x.Id));

            return modules;
        }
    }
}
