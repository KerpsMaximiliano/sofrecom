using System.Collections.Generic;

namespace Sofco.WebApi.Models.Admin
{
    public class RoleModuleFunctionalityModel
    {
        public RoleModuleFunctionalityModel()
        {
            FunctionalitiesToAdd = new List<int>();
            FunctionalitiesToRemove = new List<int>();
        }

        public int ModuleId { get; set; }

        public List<int> FunctionalitiesToAdd { get; set; }

        public List<int> FunctionalitiesToRemove { get; set; }
    }
}
