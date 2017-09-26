using System.Collections.Generic;

namespace Sofco.WebApi.Models.Admin
{
    public class RolesModel
    {
        public RolesModel()
        {
            roles = new List<int>();
        }

        public List<int> roles { get; set; }
    }
}
