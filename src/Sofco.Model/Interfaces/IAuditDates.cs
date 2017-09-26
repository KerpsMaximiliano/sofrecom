using System;

namespace Sofco.Model.Interfaces
{
    public interface IAuditDates
    {
        DateTime StartDate { get; set; }
        DateTime? EndDate { get; set; }
    }
}
