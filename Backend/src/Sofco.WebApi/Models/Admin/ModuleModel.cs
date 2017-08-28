using Sofco.Model;
using Sofco.Model.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sofco.Model.Models.Admin;

namespace Sofco.WebApi.Models.Admin
{
    public class ModuleModel : BaseEntity
    {
        public ModuleModel()
        {

        }

        public ModuleModel(Module module)
        {
            Id = module.Id;
            Description = module.Description;
            Active = module.Active;
            Code = module.Code;

            Functionalities = new List<FunctionalityModel>();
        }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        [MaxLength(5)]
        public string Code { get; set; }
        public bool Active { get; set; }
        public IList<FunctionalityModel> Functionalities { get; set; }

        public void ApplyTo(Module data)
        {
            data.Description = Description;
            data.Active = Active;
            data.Code = Code;
        }
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
