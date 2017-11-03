using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models.Admin;

namespace Sofco.WebApi.Models.Admin
{
    public class UserModel : BaseEntity, IAuditDates
    {
        public UserModel()
        {
        }

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

        [Required(ErrorMessage = "admin/user.nameRequired")]
        public string Name { get; set; }

        [Required(ErrorMessage = "admin/user.usernameRequired")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "admin/user.emailRequired")]
        public string Email { get; set; }

        public bool Active { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public IList<GroupModel> Groups { get; set; }

        internal User CreateDomain()
        {
            var user = new User();

            user.Name = this.Name;
            user.UserName = this.UserName;
            user.Email = this.Email;
            user.Active = true;
            user.StartDate = DateTime.Now;

            return user;
        }
    }

    public class UserDetailModel : UserModel
    {
        public UserDetailModel(User user) : base(user)
        {
            Roles = new List<SelectListItem>();
            Modules = new List<ModuleModelDetail>();
            Groups = new List<SelectListItem>();
        }

        public new IList<SelectListItem> Groups { get; set; }

        public IList<SelectListItem> Roles { get; set; }

        public IList<ModuleModelDetail> Modules { get; set; }
    }
}
