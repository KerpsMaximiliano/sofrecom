using Sofco.Model;
using Sofco.Model.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Models
{
    public class RoleModel : BaseEntity
    {
        public RoleModel()
        {

        }
           
        public RoleModel(Role rol)
        {
            Id = rol.Id;
            Description = rol.Description;
            Active = rol.Active;

            Groups = new List<GroupModel>();
        }

        public string Description { get; set; }

        public bool Active { get; set; }

        public IList<GroupModel> Groups { get; set; }

        public void ApplyTo(Role rol)
        {
            rol.Id = Id;
            rol.Description = Description;
            rol.Active = Active;
        }
    }
}
