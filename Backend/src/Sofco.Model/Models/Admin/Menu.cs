using System.Collections.Generic;
using Sofco.Model.Interfaces;

namespace Sofco.Model.Models.Admin
{
    public class Menu : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }

        public string Url { get; set; }

        public string Code { get; set; }

        public bool Active { get; set; }

        public IList<Module> Modules { get; set; }
    }
}
