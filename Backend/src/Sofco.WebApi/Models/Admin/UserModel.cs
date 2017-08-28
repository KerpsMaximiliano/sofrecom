using Sofco.Model;
using System.Collections.Generic;
using Sofco.Model.Models;
using Sofco.Model.Interfaces;
using System;
using Sofco.Model.Models.Admin;

namespace Sofco.WebApi.Models.Admin
{
    public class UserModel : BaseEntity, IAuditDates
    {
        public UserModel(User user)
        {
            Id = user.Id;
            Name = user.Name;
            UserName = user.UserName;
            Email = user.Email;
            Active = user.Active;
            StartDate = user.StartDate;
            EndDate = user.EndDate;

            Groups = new List<GroupModel>();
        }

        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public IList<GroupModel> Groups { get; set; }
    }

    public class UserDetailModel : UserModel
    {
        public UserDetailModel(User user) : base(user)
        {
            Roles = new List<Option<int>>();
            Modules = new List<ModuleModelDetail>();
            Groups = new List<Option<int>>();
        }

        public new IList<Option<int>> Groups { get; set; }
        public IList<Option<int>> Roles { get; set; }
        public IList<ModuleModelDetail> Modules { get; set; }
    }
}
