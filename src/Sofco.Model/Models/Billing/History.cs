using System;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Models.Billing
{
    public class History : BaseEntity
    {
        public DateTime CreatedDate { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public string Comment { get; set; }
    }
}
