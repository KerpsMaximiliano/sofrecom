using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sofco.WebApi.Models.Admin
{
    public class MenuModel
    {
        public string Description { get; set; }
        public string Functionality { get; set; }
        public string Module { get; internal set; }
    }

    public class MenuResponse
    {
        public MenuResponse()
        {
            Menus = new Collection<MenuModel>();
        }

        public ICollection<MenuModel> Menus { get; set; }
        public bool IsDirector { get; set; }
        public bool IsDaf { get; set; }
        public bool IsCdg { get; set; }
    }
}
