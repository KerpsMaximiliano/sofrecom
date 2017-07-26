using System.Collections.Generic;

namespace Sofco.WebApi.Models
{
    public class UserGroupModel
    {
        public UserGroupModel()
        {
            GroupsToAdd = new List<int>();
            GroupsToRemove = new List<int>();
        }

        public List<int> GroupsToAdd { get; set; }

        public List<int> GroupsToRemove { get; set; }
    }
}
