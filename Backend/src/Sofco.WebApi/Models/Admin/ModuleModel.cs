using Sofco.Model;
using Sofco.Model.Models;
using System.Collections.Generic;

namespace Sofco.WebApi.Models.Admin
{
    public class ModuleModel : BaseEntity
    {
        public ModuleModel(Module module)
        {
            Id = module.Id;
            Description = module.Description;
            Active = module.Active;
            Code = module.Code;

            Functionalities = new List<FunctionalityModel>();
        }

        public string Description { get; set; }
        public string Code { get; set; }
        public bool Active { get; set; }
        public IList<FunctionalityModel> Functionalities { get; set; }
    }

    public class ModuleModelDetail
    {
        public ModuleModelDetail(Module module)
        {
            Code = module.Code;
            Description = module.Description;
            Functionalities = new List<Option<string>>();
        }

        public string Code { get; set; }
        public string Description { get; set; }
        public IList<Option<string>> Functionalities { get; set; }
    }
}
