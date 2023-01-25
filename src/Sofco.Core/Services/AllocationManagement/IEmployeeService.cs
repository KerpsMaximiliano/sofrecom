using Sofco.Domain.Utils;
using System.Collections.Generic;
using Sofco.Core.Models;
using Sofco.Core.Models.AdvancementAndRefund.Refund;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Rrhh;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Core.Models.AdvancementAndRefund.Common;

namespace Sofco.Core.Services.AllocationManagement
{
    public interface IEmployeeService
    {
        ICollection<Employee> GetAll();

        IList<Employee> GetEveryone();

        Response<EmployeeModel> GetById(int id);

        Response<string> GetSectorName(int id);

        Response<EmployeeModel> GetByMail(string email);

        Response<ICollection<Employee>> Search(EmployeeSearchParams parameters);
        
        Response SendUnsubscribeNotification(EmployeeEndNotificationModel model);

        Response<EmployeeProfileModel> GetProfile(int id);

        Response FinalizeExtraHolidays(int id);

        Response AddCategories(EmployeeAddCategoriesParams parameters);

        ICollection<Option> GetAnalytics(int id);

        Response<IList<EmployeeCategoryOption>> GetCategories(int id);

        Response Update(int id, EmployeeBusinessHoursParams model);

        IList<UnemployeeListItemModel> GetUnemployees(UnemployeeSearchParameters parameters);

        Response<IList<EmployeeCategoryOption>> GetCurrentCategories();

        Response<EmployeeWorkingPendingHoursModel> GetPendingWorkingHours(int employeeId);

        Response AddExternal(AddExternalModel model);

        Response<List<Option>> GetEmployeeOptionByCurrentManager();

        Response<IList<EmployeeAdvancementDetail>> GetAdvancements(int id);

        Response<IList<EmployeeRefundDetail>> GetRefunds(int id);

        Response<IList<EmployeeCurrentAccount>> GetCurrentAccount(int id);

        Response<byte[]> GetReport();

        Response UpdateAssingComment(UpdateAssingCommentModel model);

        ICollection<Employee> GetAllForWorkTimeReport();

        Response<IList<ReportUpdownItemModel>> GetUpdownReport(ReportUpdownParameters parameters);

        Response<byte[]> GetShortReport();
    }
}
