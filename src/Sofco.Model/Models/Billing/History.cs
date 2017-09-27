using System;
using Sofco.Model.Models.Admin;

namespace Sofco.Model.Models.Billing
{
    public class History : BaseEntity
    {
        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Comment { get; set; }
    }
}
