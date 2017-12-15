using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Admin
{
    public class RoleValidationHelper
    {
        public static void ValidateIfDescriptionExist(Response<Role> response, IRoleRepository roleRepository, Role role)
        {
            if (roleRepository.ExistByDescription(role.Description, role.Id))
            {
                response.Messages.Add(new Message(Resources.Admin.Role.DescriptionExist, MessageType.Error));
            }
        }
    }
}
