using Sofco.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace Sofco.Model.Models
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
