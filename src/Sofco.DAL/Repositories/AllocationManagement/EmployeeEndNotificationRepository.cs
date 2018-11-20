using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.AllocationManagement;
using Sofco.Core.Models.AllocationManagement;
using Sofco.DAL.Repositories.Common;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.DAL.Repositories.AllocationManagement
{
    public class EmployeeEndNotificationRepository : BaseRepository<EmployeeEndNotification>, IEmployeeEndNotificationRepository
    {
        public EmployeeEndNotificationRepository(SofcoContext context) : base(context)
        {
        }

        public void Save(EmployeeEndNotification employeeEndNotification)
        {
            Insert(employeeEndNotification);

            context.SaveChanges();
        }

        public List<EmployeeEndNotification> GetByParameters(EmployeeEndNotificationParameters parameters)
        {
            IQueryable<EmployeeEndNotification> query = context.EmployeeEndNotifications;

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(s => s.EndDate.Date >= parameters.StartDate.Value.Date);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(s => s.EndDate.Date <= parameters.EndDate.Value.Date);
            }

            if (parameters.EmployeeId.HasValue)
            {
                query = query.Where(s => s.EmployeeId == parameters.EmployeeId.Value);
            }

            if (parameters.ApplicantUserId.HasValue)
            {
                query = query.Where(s => s.ApplicantUserId == parameters.ApplicantUserId.Value);
            }

            return query.ToList();
        }
    }
}
