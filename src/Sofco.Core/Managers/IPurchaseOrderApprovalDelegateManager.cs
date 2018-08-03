﻿using System.Collections.Generic;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Managers
{
    public interface IPurchaseOrderApprovalDelegateManager
    {
        List<Role> GetDelegatedRoles();
    }
}