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
            this.Id = userGroup.Id;
            this.Description = userGroup.Description;

            if(userGroup.Role != null)
            {
                this.Role = new RoleModel();
                this.Role.Id = userGroup.Role.Id;
                this.Role.Description = userGroup.Role.Description;
                this.Role.Position = userGroup.Role.Position;
            }
        }

        public string Description { get; set; }

        public RoleModel Role { get; set; }

        public void ApplyTo(UserGroup item)
        {
            item.Description = this.Description;
        }
    }
}
