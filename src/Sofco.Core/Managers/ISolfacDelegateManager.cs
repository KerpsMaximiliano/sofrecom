using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Managers
{
    public interface ISolfacDelegateManager
    {
        List<Role> GetDelegatedRoles();
    }
}