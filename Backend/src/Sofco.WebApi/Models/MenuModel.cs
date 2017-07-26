using Sofco.Model;
using Sofco.Model.Models;

namespace Sofco.WebApi.Models
{
    public class MenuModel : BaseEntity
    {
        public MenuModel(Menu menu)
        {
            Id = menu.Id;
            Description = menu.Description;
            Url = menu.Url;
            Code = menu.Code;
        }

        public string Description { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }
    }
}
