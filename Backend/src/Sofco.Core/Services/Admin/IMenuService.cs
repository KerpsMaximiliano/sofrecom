using Sofco.Model.Models;
using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Services.Admin
{
    public interface IMenuService
    {
        IList<Menu> GetMenu(string userId);
    }
}
