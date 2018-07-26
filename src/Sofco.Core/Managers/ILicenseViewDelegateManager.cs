using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Managers
{
    public interface ILicenseViewDelegateManager
    {
        List<Role> GetDelegatedRoles();
    }
}