using Sofco.Model.Utils;
using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Model.DTO;
using Sofco.Model.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();

        Response<EmployeeModel> GetById(int id);

        Response<ICollection<Employee>> Search(EmployeeSearchParams parameters);

        Response SendUnsubscribeNotification(string employeeName, UnsubscribeNotificationParams parameters);

        Response<EmployeeProfileModel> GetProfile(int id);

        Response FinalizeExtraHolidays(int id);

        Response AddCategories(EmployeeAddCategoriesParams parameters);

        ICollection<Option> GetAnalytics(int id);
    }
}
