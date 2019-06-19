using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Views;
using Sofco.Domain.Models.Reports;

namespace Sofco.DAL.Repositories.Reports
{
    public class EmployeesViewRepository : IEmployeeViewRepository
    {
        private readonly DbSet<EmployeeView> employeeViews;

        public EmployeesViewRepository(ReportContext context)
        {
            employeeViews = context.Set<EmployeeView>();
        }


    }
}
