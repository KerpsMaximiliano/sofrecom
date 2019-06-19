using System.Collections.Generic;
using Sofco.Domain.Models.Reports;

namespace Sofco.Core.DAL.Views
{
    public interface IEmployeeViewRepository
    {
        IList<EmployeeView> Get();
    }
}
