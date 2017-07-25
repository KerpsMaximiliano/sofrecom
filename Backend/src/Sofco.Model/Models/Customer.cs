using Sofco.Model.Interfaces;

namespace Sofco.Model.Models
{
    public class Customer : BaseEntity, ILogicalDelete
    {
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}
