using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Relationships
{
    public class UserGroup
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
