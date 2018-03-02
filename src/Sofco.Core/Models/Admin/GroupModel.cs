using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Models.Admin
{
    public class GroupModel : BaseEntity, IAuditDates
    {
        public GroupModel()
        {
        }

        public GroupModel(Group group)
        {
            Id = group.Id;
            Description = group.Description;
            Active = group.Active;
            StartDate = group.StartDate;
            EndDate = group.EndDate;
            Email = group.Email;

            Users = new List<UserModel>();
        }

        [Required(ErrorMessage = "admin/group.descriptionRequired")]
        [MaxLength(50, ErrorMessage = "admin/group.wrongDescriptionLength")]
        public string Description { get; set; }

        [EmailAddress(ErrorMessage = "admin/group.wrongEmail")]
        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public RoleModel Role { get; set; }

        public int RoleId { get; set; }

        public List<UserModel> Users { get; set; }

        public void ApplyTo(Group group)
        {
            group.Id = Id;
            group.Description = Description;
            group.Active = Active;
            group.Email = Email;
        }
    }

    public class GroupModelDetail : BaseEntity
    {
        public GroupModelDetail(Group group)
        {
            Id = group.Id;
            Description = group.Description;
        }

        public string Description { get; set; }
    }
}
