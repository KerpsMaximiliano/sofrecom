using Sofco.Model.Models;
using System.Collections.Generic;
using Sofco.Model.Models.Admin;

namespace Sofco.WebApi.Models.Admin
{
    public class MenuModel
    {
        public MenuModel(Menu menu)
        {
            Description = menu.Description;
            Url = menu.Url;
            Code = menu.Code;

            Modules = new List<ModuleModelDetail>();
        }

        public string Description { get; set; }
        public string Url { get; set; }
        public string Code { get; set; }

        public List<ModuleModelDetail> Modules;
    }
}
