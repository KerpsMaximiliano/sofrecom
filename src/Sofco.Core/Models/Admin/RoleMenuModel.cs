using System.Collections.Generic;

namespace Sofco.Core.Models.Admin
{
    public class RoleMenuModel
    {
        public RoleMenuModel()
        {
            MenusToAdd = new List<int>();
            MenusToRemove = new List<int>();
        }

        public List<int> MenusToAdd { get; set; }

        public List<int> MenusToRemove { get; set; }
    }
}
