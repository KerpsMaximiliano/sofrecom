using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();

        Response<Employee> GetById(int id);

        Response<ICollection<Employee>> Search(EmployeeSearchParams parameters);

        Response SendUnsubscribeNotification(string employeeName, UnsubscribeNotificationParams parameters);
        Response<EmployeeProfileDto> GetProfile(int id);
    }
}
