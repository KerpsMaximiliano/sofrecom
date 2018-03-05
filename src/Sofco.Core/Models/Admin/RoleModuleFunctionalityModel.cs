using System.Collections.Generic;

namespace Sofco.Core.Models.Admin
{
    public class RoleModuleFunctionalityModel
    {
        public RoleModuleFunctionalityModel()
        {
            FunctionalitiesToAdd = new List<int>();
            FunctionalitiesToRemove = new List<int>();
        }

        public List<int> FunctionalitiesToAdd { get; set; }

        public List<int> FunctionalitiesToRemove { get; set; }
    }
}
