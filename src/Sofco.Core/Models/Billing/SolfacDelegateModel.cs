using System;

namespace Sofco.Core.Models.Billing
{
    public class SolfacDelegateModel
    {
        public int Id { get; set; }

        public string ManagerName { get; set; }

        public string ServiceName { get; set; }

        public string UserName { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? Created { get; set; }
    }
}
