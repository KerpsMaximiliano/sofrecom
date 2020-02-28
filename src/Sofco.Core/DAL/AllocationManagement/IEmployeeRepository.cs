using System;
using System.Collections;
using System.Collections.Generic;
using Sofco.Core.DAL.Common;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.WorkTimeManagement;
using Sofco.Domain.DTO;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Relationships;
using Sofco.Domain.Utils;

namespace Sofco.Core.DAL.AllocationManagement
{
    public interface IEmployeeRepository : IBaseRepository<Employee>
    {
        new ICollection<Employee> GetAll();

        bool Exist(int employeeId);

        Employee GetById(int id);

        List<Employee> GetById(int[] ids);

        Employee GetByEmail(string email);

        void Update(List<Employee> employees);

        List<Employee> GetByEndDate(DateTime today);

        List<Employee> GetByEmployeeNumber(string[] employeeNumbers);

        Employee GetByEmployeeNumber(string employeeNumber);

        void UpdateEndDate(Employee employeeToChange);

        ICollection<Employee> Search(EmployeeSearchParams parameters, DateTime startDate, DateTime endDate);

        void ResetAllExamDays();

        IList<EmployeeCategory> GetEmployeeCategories(int employeeId);

        void UpdateBusinessHours(Employee employee);

        IList<Employee> GetByEmployeeNumbers(IEnumerable<string> employeeNumbers);

        IList<Employee> SearchUnemployees(UnemployeeSearchParameters parameters);

        void Save(List<Employee> employees);

        void Save(Employee employee);

        IList<Employee> GetByAnalyticWithWorkTimes(int analyticId);

        Employee GetUserInfo(string email);

        IList<Employee> GetUnassignedBetweenDays(DateTime startDate, DateTime endDate, string appSettingAnalyticBank);

        IList<Employee> GetByAnalyticIds(List<int> analyticIds);

        IList<Sector> GetAnalyticsWithSector(int employeeId);

        IList<Employee> GetByManagerId(int managerId);
        Employee GetByDocumentNumber(int dni);
        IList<Employee> GetMissingEmployess(IList<int> prepaidImportedDataIds);

        IList<Tuple<int, string, string>> GetIdAndEmployeeNumber(int year, int month);
        Employee GetWithSocialChargesAndAllocations(int employeeId);

        Employee GetByEmailWithDiscounts(string modelEmail);
        IList<Employee> GetByAnalyticWithSocialCharges(int idAnalytic, DateTime startDate, DateTime endDate);
        void UpdateAssignComments(Employee employee);
        IList<Employee> GetUnassignedBetweenDays(DateTime startDate, DateTime endDate, int employeeId);
        ICollection<Employee> GetAllForWorkTimeReport();
        IList<Employee> GetByAnalyticIdInCurrentDate(int analyticId);
        void UpdateManager(int id, int managerId);
        SocialCharge GetSocialCharges(int employeeId, DateTime date);
        IList<Employee> GetUpdownReport(ReportUpdownParameters parameters);
        IList<Employee> GetWithHolidaysPendingGreaterThen35();
        IList<Employee> GetOnTestPeriod(DateTime date);
        void UpdateOnTestPeriod(Employee item);
    }
}
