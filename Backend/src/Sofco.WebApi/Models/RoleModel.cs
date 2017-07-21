using Sofco.Model;
using System.Collections.Generic;
using Sofco.Model.Models;

namespace Sofco.WebApi.Models
{
    public class RoleModel : BaseEntity
    {
        public RoleModel()
        {
        }

        public RoleModel(Role role)
        {
            this.Id = role.Id;
            this.Description = role.Description;
            this.Position = role.Position;

            if(role.UserGroups != null)
            {
                this.UserGroups = new List<UserGroupModel>();

                foreach (var item in role.UserGroups)
                {
                    item.Role = null;
                    this.UserGroups.Add(new UserGroupModel(item));
                }
            }
        }

        public string Description { get; set; }

        public string Position { get; set; }

        public IList<UserGroupModel> UserGroups {get;set;}

        public void ApplyTo(Role item)
        {
            item.Description = this.Description;
            item.Position = this.Position;
        }
    }
}
