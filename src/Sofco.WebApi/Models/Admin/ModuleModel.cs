using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Model;
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

            Functionalities = new List<FunctionalityModel>();
        }

        [Required(ErrorMessage = "admin/module.descriptionRequired")]
        [MaxLength(50, ErrorMessage = "admin/module.wrongDescriptionLength")]
        public string Description { get; set; }

        public bool Active { get; set; }

        public IList<FunctionalityModel> Functionalities { get; set; }

        public void ApplyTo(Module data)
        {
            data.Description = Description;
            data.Active = Active;
        }
    }

    public class ModuleModelDetail
    {
        public ModuleModelDetail(Module module)
        {
            Code = module.Code;
            Description = module.Description;
            Functionalities = new List<SelectListItem>();
        }

        public string Code { get; set; }

        public string Description { get; set; }

        public IList<SelectListItem> Functionalities { get; set; }
    }
}
