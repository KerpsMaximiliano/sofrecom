namespace Sofco.Core.Models.Admin
{
    public class UserSelectListItem : SelectListModel
    {
        public string UserName { get; set; }

        public string Email { get; set; }

        public string ExternalId { get; set; }
    }
}
