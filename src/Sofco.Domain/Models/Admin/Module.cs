using System.Collections.Generic;
using Sofco.Common.Domains;

namespace Sofco.Domain.Models.Admin
{
    public class Module : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public bool Active { get; set; }

        public string Code { get; set; }

        public IList<Functionality> Functionalities { get; set; }
    }
}
