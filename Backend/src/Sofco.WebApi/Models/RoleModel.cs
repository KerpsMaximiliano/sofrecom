using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sofco.WebApi.Models
{
    public class RoleModel : BaseEntity, IAuditDates, IEquatable<RoleModel>
    {
        public RoleModel()
        {

        }
           
        public RoleModel(Role rol)
        {
            Id = rol.Id;
            Description = rol.Description;
            Active = rol.Active;
            StartDate = rol.StartDate;
            EndDate = rol.EndDate;

            Groups = new List<GroupModel>();
            Modules = new List<ModuleModel>();
        }

        [Required(ErrorMessage = "El campo descripción es requerido")]
        [MaxLength(50, ErrorMessage = "El campo descripción debe tener un maximo de 50 caracteres")]
        public string Description { get; set; }

        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<GroupModel> Groups { get; set; }
        public IList<ModuleModel> Modules { get; set; }

        public void ApplyTo(Role rol)
        {
            rol.Id = Id;
            rol.Description = Description;
            rol.Active = Active;
        }

        public bool Equals(RoleModel other)
        {
            return this.Id == other.Id;
        }
    }
}
