using Sofco.Model.Models;
using System.Collections.Generic;

namespace Sofco.Core.Services
{
    public interface IMenuService
    {
        IList<Menu> GetMenuByRoleId(int[] roleIds);
    }
}
