using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sofco.WebApi.Models.Admin
{
    public class UserSelectListItem : SelectListItem
    {
        public string UserName { get; set; }
    }
}
