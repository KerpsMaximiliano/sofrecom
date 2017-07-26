using Sofco.Model;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Interfaces;
using System;

namespace Sofco.WebApi.Models
{
    public class UserModel : BaseEntity, IAuditDates
    {
        public UserModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            Email = user.Email;
            Active = user.Active;
            StartDate = user.StartDate;
            EndDate = user.EndDate;

            Groups = new List<GroupModel>();
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<GroupModel> Groups { get; set; }
    }
}
