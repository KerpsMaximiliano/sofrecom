using System.Collections.Generic;

namespace Sofco.WebApi.Models
{
    public class RoleFunctionalityModel
    {
        public RoleFunctionalityModel()
        {
            FunctionlitiesToAdd = new List<int>();
            FunctionlitiesToRemove = new List<int>();
        }

        public List<int> FunctionlitiesToAdd { get; set; }

        public List<int> FunctionlitiesToRemove { get; set; }
    }
}
