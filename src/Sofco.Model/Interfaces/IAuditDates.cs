using System;

namespace Sofco.Domain.Interfaces
{
    public interface IAuditDates
    {
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
}
