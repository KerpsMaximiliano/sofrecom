using Sofco.Common.Domains;

namespace Sofco.Domain.Utils
{
    public class Option : BaseEntity, ILogicalDelete
    {
        public string Text { get; set; }

        public bool Active { get; set; }
    }
}
