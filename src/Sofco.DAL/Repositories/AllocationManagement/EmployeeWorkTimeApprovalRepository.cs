using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeWorkTimeApprovalRepository : BaseRepository<Employee>, IEmployeeWorkTimeApprovalRepository
    {
        public EmployeeWorkTimeApprovalRepository(SofcoContext context) : base(context)
        {
        }

        public List<EmployeeWorkTimeApproval> Get()
        {
            var result = context
                .Employees
                .Where(x => x.EndDate == null)
                .Include(x => x.Allocations)
                .ThenInclude(x => x.Analytic)
                .ThenInclude(x => x.Manager)
                .GroupJoin(
                    context
                    .WorkTimeApprovals
                    .Include(x => x.ApprovalUser),
                    e => e.Id,
                    w => w.EmployeeId,
                    Translate);

            return result.ToList();
        }

        public EmployeeWorkTimeApproval Translate(Employee employee, IEnumerable<WorkTimeApproval> workTimeApprovals)
        {
            var allocation = employee.Allocations.LastOrDefault();

            var managerName = allocation?.Analytic?.Manager?.Name;

            var firstWorkTimeApproval = workTimeApprovals.FirstOrDefault();

            return new EmployeeWorkTimeApproval
            {
                EmployeeId = employee.Id.ToString(),
                Name = employee.Name,
                Client = allocation?.Analytic?.ClientExternalName,
                Service = allocation?.Analytic?.Service,
                Manager = managerName,
                ApprovalName = firstWorkTimeApproval?.ApprovalUser?.Name,
                WorkTimeApproval = firstWorkTimeApproval
            };
        }
    }
}
