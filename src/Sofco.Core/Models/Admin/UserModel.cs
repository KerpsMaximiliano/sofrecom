using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sofco.Domain;
using Sofco.Domain.Interfaces;
using Sofco.Domain.Models.Admin;

namespace Sofco.Core.Models.Admin
{
    public class UserModel : BaseEntity, IAuditDates
    {
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

        public int ManagerId { get; set; }
        public bool IsExternal { get; set; }

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
