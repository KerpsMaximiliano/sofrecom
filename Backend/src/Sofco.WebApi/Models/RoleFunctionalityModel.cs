using System.Collections.Generic;

namespace Sofco.WebApi.Models
{
    public class RoleFunctionalityModel
    {
        public RoleFunctionalityModel()
        {
            FunctionalitiesToAdd = new List<int>();
            FunctionalitiesToRemove = new List<int>();
        }

        public List<int> FunctionalitiesToAdd { get; set; }

        public List<int> FunctionalitiesToRemove { get; set; }
    }
}
