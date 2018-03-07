using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sofco.Model;
using Sofco.Model.Interfaces;
using Sofco.Model.Models.Admin;

namespace Sofco.Core.Models.Admin
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

        public int EmployeeId { get; set; }

        public User CreateDomain()
        {
            var user = new User
            {
                Name = Name,
                UserName = UserName,
                Email = Email,
                Active = true,
                StartDate = DateTime.Now
            };

            return user;
        }
    }
}
