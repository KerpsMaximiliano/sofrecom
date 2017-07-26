using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models;
using System;
using System.Collections.Generic;

namespace Sofco.WebApi.Models
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

            Users = new List<UserModel>();
        }

        public string Description { get; set; }

        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public RoleModel Role { get; set; }

        public List<UserModel> Users { get; set; }

        public void ApplyTo(Group group)
        {
            group.Id = Id;
            group.Description = Description;
            group.Active = Active;

            if(Role != null)
            {
                group.Role = new Role();
                group.Role.Id = Role.Id;
            }
        }
    }
}
