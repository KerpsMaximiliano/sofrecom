using Sofco.Model;
using Sofco.Model.Models;

namespace Sofco.WebApi.Models
{
    public class UserGroupModel : BaseEntity
    {
        public UserGroupModel()
        {

        }

        public UserGroupModel(UserGroup userGroup)
        {
            Id = userGroup.Id;
            Description = userGroup.Description;
            Active = userGroup.Active;
        }

        public string Description { get; set; }

        public bool Active { get; set; }

        public RoleModel Role { get; set; }

        public void ApplyTo(UserGroup userGroup)
        {
            userGroup.Id = Id;
            userGroup.Description = Description;
            userGroup.Active = Active;

            if(Role != null)
            {
                userGroup.Role = new Role();
                userGroup.Role.Id = Role.Id;
            }
        }
    }
}
