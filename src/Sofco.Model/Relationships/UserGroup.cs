using Sofco.Model.Models;
using Sofco.Model.Models.Admin;

namespace Sofco.Model.Relationships
{
    public class UserGroup
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
