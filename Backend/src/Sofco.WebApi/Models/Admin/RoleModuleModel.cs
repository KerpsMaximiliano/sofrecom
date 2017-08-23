using System.Collections.Generic;

namespace Sofco.WebApi.Models.Admin
{
    public class RoleModuleModel
    {
        public RoleModuleModel()
        {
            ModulesToAdd = new List<int>();
            ModulesToRemove = new List<int>();
        }

        public List<int> ModulesToAdd { get; set; }
        public List<int> ModulesToRemove { get; set; }
    }
}
