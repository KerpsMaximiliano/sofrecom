using System;
using System.Collections.Generic;
using System.Linq;
using Hangfire;
using Microsoft.Extensions.Options;
using Sofco.Core.Services.Admin;
using Sofco.Core.Services.Jobs;
using Sofco.Domain.Models.Admin;
using Sofco.Service.Settings.Jobs;
using Sofco.WebJob.Helpers;
using Sofco.WebJob.Jobs.Interfaces;
using Sofco.WebJob.Services.Interfaces;

namespace Sofco.WebJob.Services
{
    public class JobService : IJobService
    {
        private const string LicenseCertificatePendingDayOfMonthKey = "LicenseCertificatePendingDayOfMonth";

        private readonly JobSetting jobSetting;

        private readonly ISettingService settingService;

        public JobService(IOptions<JobSetting> jobSetting, ISettingService settingService)
        {
            this.settingService = settingService;
            this.jobSetting = jobSetting.Value;
        }

        public void Execute()
        {
            var jobSettings = settingService.GetJobSettings().Data;

            Init(jobSetting.LocalTimeZoneName, jobSettings);
        }

        private void Init(string timeZoneName, List<Setting> settings)
        {
            ClearJobs();

            var localTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneName);

            RecurringJob.AddOrUpdate<IAzureJob>(JobNames.Azure, j => j.Execute(), Cron.Weekly(DayOfWeek.Monday), localTimeZone);

            RecurringJob.AddOrUpdate<IEmployeeSyncActionJob>(JobNames.TigerEmployeeSync, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<ICustomerUpdateJob>(JobNames.CustomerUpdate, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IServiceUpdateJob>(JobNames.ServiceUpdate, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IProjectUpdateJob>(JobNames.ProjectUpdate, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IContactUpdateJobService>(JobNames.ContactUpdate, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IOpportunityUpdateJobService>(JobNames.OpportunityUpdate, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IAllocationCleanJob>(JobNames.AllocationClean, j => j.Execute(), Cron.Daily(7), localTimeZone);

            RecurringJob.AddOrUpdate<IHealthInsuranceSyncJob>(JobNames.TigerHealthInsurance, j => j.Execute(), Cron.Weekly(DayOfWeek.Monday, 10), localTimeZone);

            RecurringJob.AddOrUpdate<IEmployeeResetExamDaysJob>(JobNames.EmployeeResetExamDays, j => j.Execute(), Cron.Yearly(12, 31, 9), localTimeZone);

            RecurringJob.AddOrUpdate<ILicenseDaysUpdateJob>(JobNames.LicenseDaysUpdate, j => j.Execute(), Cron.Yearly(10, 1, 9), localTimeZone);
             
            BackgroundJob.Schedule<IEmployeeForceSaveJob>(j => j.Execute(), DateTime.UtcNow.AddYears(100));

            var licenseCertificatePendingDayOfMonth = settings.SingleOrDefault(s => s.Key == LicenseCertificatePendingDayOfMonthKey);

            var licenseCertificatePending = licenseCertificatePendingDayOfMonth != null ? Cron.Monthly(int.Parse(licenseCertificatePendingDayOfMonth.Value)) : Cron.Monthly(25);

            RecurringJob.AddOrUpdate<ILicenseCertificatePendingJob>(JobNames.LicenseCertificatePending, j => j.Execute(), licenseCertificatePending, localTimeZone);

            RecurringJob.AddOrUpdate<IEmployeeSyncProfileJobService>(JobNames.EmployeeProfileHistory, j => j.Sync(true), Cron.Daily(8, 0), localTimeZone);

            RecurringJob.AddOrUpdate<IApplicantInterviewNotificationBeforeService>(JobNames.ApplicantInterviewNotificationBefore, j => j.Execute(), Cron.Daily(8, 0), localTimeZone);

            RecurringJob.AddOrUpdate<IApplicantInterviewNotificationAfterService>(JobNames.ApplicantInterviewNotificationAfter, j => j.Execute(), Cron.Daily(8, 0), localTimeZone);

            RecurringJob.AddOrUpdate<ISocialChargesJob>(JobNames.SocialCharges, j => j.Execute(), Cron.Monthly(DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month), 8), localTimeZone);

            RecurringJob.AddOrUpdate<IPowerBiJob>(JobNames.PowerBi, j => j.Execute(), Cron.Hourly(5), localTimeZone);
        }

        private void ClearJobs()
        {
            JobHelper.ClearAllRecurringJob();

            JobHelper.ClearScheduledJob(typeof(IEmployeeForceSaveJob));

            JobHelper.ClearScheduledJob(typeof(ILicenseDaysUpdateJob));
        }
    }
}
