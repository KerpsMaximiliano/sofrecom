using System.Collections.Generic;

namespace Sofco.Core.Models.Admin
{
    public class RolesModel
    {
        public RolesModel()
        {
            Roles = new List<int>();
        }

        public List<int> Roles { get; set; }
    }
}
