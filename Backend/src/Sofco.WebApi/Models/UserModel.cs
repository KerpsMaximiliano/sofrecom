using Sofco.Model;
using System.Collections.Generic;
using Sofco.Model.Models;

namespace Sofco.WebApi.Models
{
    public class UserModel : BaseEntity
    {
        public UserModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Active = user.Active;

            Groups = new List<GroupModel>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }

        public IList<GroupModel> Groups { get; set; }
    }
}
