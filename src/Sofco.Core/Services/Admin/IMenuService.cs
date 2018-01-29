using System.Collections.Generic;
using Sofco.Model.Models.Admin;
using Sofco.Model.Relationships;

namespace Sofco.Core.Services.Admin
{
    public interface IMenuService
    {
        IList<RoleFunctionality> GetFunctionalitiesByUserName(string userId);
        string GetGroupMail(string emailConfigDafCode);
    }
}
