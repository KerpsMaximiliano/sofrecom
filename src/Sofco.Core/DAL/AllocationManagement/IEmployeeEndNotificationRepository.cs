using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeEndNotificationRepository
    {
        void Save(EmployeeEndNotification employeeEndNotification);

        List<EmployeeEndNotification> GetByParameters(EmployeeEndNotificationParameters parameters);
    }
}