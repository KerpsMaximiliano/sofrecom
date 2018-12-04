using System;
using Sofco.Domain.Models.Admin;

namespace Sofco.Domain.Interfaces
{
    public interface IAudit
    {
        DateTime CreatedAt { get; set; }

        DateTime ModifiedAt { get; set; }

        int CreatedById { get; set; }
        User CreatedBy { get; set; }

        int ModifiedById { get; set; }
        User ModifiedBy { get; set; }
    }
}
