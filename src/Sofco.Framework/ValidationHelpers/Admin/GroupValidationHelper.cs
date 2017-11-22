﻿using Sofco.Core.DAL.Admin;
using Sofco.Model.Enums;
using Sofco.Model.Models.Admin;
using Sofco.Model.Utils;

namespace Sofco.Framework.ValidationHelpers.Admin
{
    public class GroupValidationHelper
    {
        public static void ValidateRol(Group group, Response<Group> response, IRoleRepository roleRepository)
        {
            if (group.Role != null)
            {
                var role = roleRepository.GetSingle(x => x.Id == group.Role.Id);

                if (role == null)
                {
                    response.Messages.Add(new Message(Resources.Admin.Role.NotFound, MessageType.Error));
                }

                group.Role = role;
            }
        }

        public static void ValidateDescription(Group group, Response<Group> response, IGroupRepository groupRepository)
        {
            if (groupRepository.DescriptionExist(group.Description, group.Id))
            {
                response.Messages.Add(new Message(Resources.Admin.Group.DescriptionAlreadyExist, MessageType.Error));
            }
        }
    }
}
