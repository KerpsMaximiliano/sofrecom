using Sofco.Domain.Utils;
using System.Collections.Generic;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();

        Response<EmployeeModel> GetById(int id);

        Response<ICollection<Employee>> Search(EmployeeSearchParams parameters);

        Response SendUnsubscribeNotification(EmployeeEndNotificationModel model);

        Response<EmployeeProfileModel> GetProfile(int id);

        Response FinalizeExtraHolidays(int id);

        Response AddCategories(EmployeeAddCategoriesParams parameters);

        ICollection<Option> GetAnalytics(int id);

        Response<IList<EmployeeCategoryOption>> GetCategories(int id);

        Response UpdateBusinessHours(int id, EmployeeBusinessHoursParams model);

        IList<UnemployeeListItemModel> GetUnemployees(UnemployeeSearchParameters parameters);

        Response<IList<EmployeeCategoryOption>> GetCurrentCategories();

        Response<EmployeeWorkingPendingHoursModel> GetPendingWorkingHours(int employeeId);

        Response AddExternal(AddExternalModel model);

        Response<List<Option>> GetEmployeeOptionByCurrentManager();
    }
}
