using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Sofco.Core.DAL.Views;
using Sofco.Domain.Models.Reports;

namespace Sofco.DAL.Repositories.Reports
{
    public class EmployeesViewRepository : IEmployeeViewRepository
    {
        private readonly DbSet<EmployeeView> employeeViews;
        private readonly ReportContext context;

        public EmployeesViewRepository(ReportContext context)
        {
            employeeViews = context.Set<EmployeeView>();
            this.context = context;
        }


        public IList<EmployeeView> Get()
        {
            var list = new List<EmployeeView>();

            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT * FROM report.ResourcesView";

                context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            list.Add(MapRow(reader));
                        }
                    }
                }

                context.Database.CloseConnection();
            }

            return list;
        }

        private EmployeeView MapRow(DbDataReader reader)
        {
            var employee = new EmployeeView();

            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeNumber")))
                employee.EmployeeNumber = reader.GetString(reader.GetOrdinal("EmployeeNumber"));

            if (!reader.IsDBNull(reader.GetOrdinal("Name")))
                employee.Name = reader.GetString(reader.GetOrdinal("Name"));

            if (!reader.IsDBNull(reader.GetOrdinal("Profile")))
                employee.Profile = reader.GetString(reader.GetOrdinal("Profile"));

            if (!reader.IsDBNull(reader.GetOrdinal("Seniority")))
                employee.Seniority = reader.GetString(reader.GetOrdinal("Seniority"));

            if (!reader.IsDBNull(reader.GetOrdinal("Technology")))
                employee.Technology = reader.GetString(reader.GetOrdinal("Technology"));

            if (!reader.IsDBNull(reader.GetOrdinal("StartDate")))
                employee.StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));

            if (!reader.IsDBNull(reader.GetOrdinal("EndDate")))
                employee.EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));

            if (!reader.IsDBNull(reader.GetOrdinal("Email")))
                employee.Email = reader.GetString(reader.GetOrdinal("Email"));

            if (!reader.IsDBNull(reader.GetOrdinal("Cuil")))
                employee.Cuil = reader.GetDecimal(reader.GetOrdinal("Cuil"));

            if (!reader.IsDBNull(reader.GetOrdinal("DocumentNumber")))
                employee.DocumentNumber = reader.GetInt32(reader.GetOrdinal("DocumentNumber"));

            if (!reader.IsDBNull(reader.GetOrdinal("Birthday")))
                employee.Birthday = reader.GetDateTime(reader.GetOrdinal("Birthday"));

            if (!reader.IsDBNull(reader.GetOrdinal("OfficeAddress")))
                employee.OfficeAddress = reader.GetString(reader.GetOrdinal("OfficeAddress"));

            if (!reader.IsDBNull(reader.GetOrdinal("Address")))
                employee.Address = reader.GetString(reader.GetOrdinal("Address"));

            if (!reader.IsDBNull(reader.GetOrdinal("Location")))
                employee.Location = reader.GetString(reader.GetOrdinal("Location"));

            if (!reader.IsDBNull(reader.GetOrdinal("Province")))
                employee.Province = reader.GetString(reader.GetOrdinal("Province"));

            if (!reader.IsDBNull(reader.GetOrdinal("Country")))
                employee.Country = reader.GetString(reader.GetOrdinal("Country"));

            if (!reader.IsDBNull(reader.GetOrdinal("PhoneAreaCode")))
                employee.PhoneAreaCode = reader.GetInt32(reader.GetOrdinal("PhoneAreaCode"));

            if (!reader.IsDBNull(reader.GetOrdinal("PhoneNumber")))
                employee.PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber"));

            if (!reader.IsDBNull(reader.GetOrdinal("HolidaysByLaw")))
                employee.HolidaysByLaw = reader.GetInt32(reader.GetOrdinal("HolidaysByLaw"));

            if (!reader.IsDBNull(reader.GetOrdinal("HolidaysPendingByLaw")))
                employee.HolidaysPendingByLaw = reader.GetInt32(reader.GetOrdinal("HolidaysPendingByLaw"));

            if (!reader.IsDBNull(reader.GetOrdinal("ExtraHolidaysQuantity")))
                employee.ExtraHolidaysQuantity = reader.GetInt32(reader.GetOrdinal("ExtraHolidaysQuantity"));

            if (!reader.IsDBNull(reader.GetOrdinal("HasExtraHolidays")))
                employee.HasExtraHolidays = reader.GetBoolean(reader.GetOrdinal("HasExtraHolidays"));

            if (!reader.IsDBNull(reader.GetOrdinal("HolidaysPending")))
                employee.HolidaysPending = reader.GetInt32(reader.GetOrdinal("HolidaysPending"));

            if (!reader.IsDBNull(reader.GetOrdinal("ExamDaysTaken")))
                employee.ExamDaysTaken = reader.GetInt32(reader.GetOrdinal("ExamDaysTaken"));

            if (!reader.IsDBNull(reader.GetOrdinal("BillingPercentage")))
                employee.BillingPercentage = reader.GetDecimal(reader.GetOrdinal("BillingPercentage"));

            if (!reader.IsDBNull(reader.GetOrdinal("BusinessHours")))
                employee.BusinessHours = reader.GetInt32(reader.GetOrdinal("BusinessHours"));

            if (!reader.IsDBNull(reader.GetOrdinal("BusinessHoursDescription")))
                employee.BusinessHoursDescription = reader.GetString(reader.GetOrdinal("BusinessHoursDescription"));

            if (!reader.IsDBNull(reader.GetOrdinal("PrepaidHealthCode")))
                employee.PrepaidHealthCode = reader.GetInt32(reader.GetOrdinal("PrepaidHealthCode"));

            if (!reader.IsDBNull(reader.GetOrdinal("EndReason")))
                employee.EndReason = reader.GetString(reader.GetOrdinal("EndReason"));

            if (!reader.IsDBNull(reader.GetOrdinal("IsExternal")))
                employee.IsExternal = reader.GetBoolean(reader.GetOrdinal("IsExternal"));

            if (!reader.IsDBNull(reader.GetOrdinal("HasCreditCard")))
                employee.HasCreditCard = reader.GetBoolean(reader.GetOrdinal("HasCreditCard"));

            if (!reader.IsDBNull(reader.GetOrdinal("Bank")))
                employee.Bank = reader.GetString(reader.GetOrdinal("Bank"));

            if (!reader.IsDBNull(reader.GetOrdinal("ManagerName")))
                employee.ManagerName = reader.GetString(reader.GetOrdinal("ManagerName"));

            if (!reader.IsDBNull(reader.GetOrdinal("Id")))
                employee.Id = reader.GetInt32(reader.GetOrdinal("Id"));

            if (!reader.IsDBNull(reader.GetOrdinal("Percentage")))
                employee.Percentage = reader.GetDecimal(reader.GetOrdinal("Percentage"));

            if (!reader.IsDBNull(reader.GetOrdinal("title")))
                employee.Title = reader.GetString(reader.GetOrdinal("title"));

            if (!reader.IsDBNull(reader.GetOrdinal("AnalyticName")))
                employee.AnalyticName = reader.GetString(reader.GetOrdinal("AnalyticName"));

            if (!reader.IsDBNull(reader.GetOrdinal("ServiceName")))
                employee.ServiceName = reader.GetString(reader.GetOrdinal("ServiceName"));

            if (!reader.IsDBNull(reader.GetOrdinal("AccountName")))
                employee.AccountName = reader.GetString(reader.GetOrdinal("AccountName"));

            if (!reader.IsDBNull(reader.GetOrdinal("ServiceTypeName")))
                employee.ServiceTypeName = reader.GetString(reader.GetOrdinal("ServiceTypeName"));

            if (!reader.IsDBNull(reader.GetOrdinal("SolutionName")))
                employee.SolutionName = reader.GetString(reader.GetOrdinal("SolutionName"));

            if (!reader.IsDBNull(reader.GetOrdinal("TechnologyName")))
                employee.TechnologyName = reader.GetString(reader.GetOrdinal("TechnologyName"));

            if (!reader.IsDBNull(reader.GetOrdinal("SoftwareLawName")))
                employee.SoftwareLawName = reader.GetString(reader.GetOrdinal("SoftwareLawName"));

            if (!reader.IsDBNull(reader.GetOrdinal("ActivityName")))
                employee.ActivityName = reader.GetString(reader.GetOrdinal("ActivityName"));

            if (!reader.IsDBNull(reader.GetOrdinal("Activity")))
                employee.Activity = reader.GetInt32(reader.GetOrdinal("Activity"));

            return employee;
        }
    }
}
