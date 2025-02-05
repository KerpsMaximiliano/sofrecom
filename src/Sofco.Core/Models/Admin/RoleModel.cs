﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sofco.Domain;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Models.Admin
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
            Code = rol.Code;

            Groups = new List<GroupModel>();
            Modules = new List<ModuleModel>();
        }

        [Required(ErrorMessage = "admin/role.descriptionRequired")]
        [MaxLength(50, ErrorMessage = "admin/role.wrongDescriptionLength")]
        public string Description { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Code { get; set; }

        public IList<GroupModel> Groups { get; set; }

        public IList<ModuleModel> Modules { get; set; }

        public void ApplyTo(Role rol)
        {
            rol.Id = Id;
            rol.Description = Description;
            rol.Active = Active;
            rol.Code = Code;
        }

        public bool Equals(RoleModel other)
        {
            return Id == other.Id;
        }
    }
}
