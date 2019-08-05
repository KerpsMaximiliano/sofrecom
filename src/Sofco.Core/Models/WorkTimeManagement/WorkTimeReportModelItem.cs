using System;
using System.Collections.Generic;

namespace Sofco.Core.Models.WorkTimeManagement
{
    public class WorkTimeReportModelItem
    {
        public string Client { get; set; }

        public string Analytic { get; set; }

        public string Manager { get; set; }

        public string MonthYear { get; set; }

        public decimal Facturability { get; set; }

        public decimal AllocationPercentage { get; set; }

        public decimal HoursMustLoad { get; set; }

        public decimal HoursLoaded { get; set; }

        public string Employee { get; set; }

        public bool Result { get; set; }

        public int EmployeeId { get; set; }

        public int AnalyticId { get; set; }

        public decimal RealPercentage { get; set; }

        public string EmployeeNumber { get; set; }

        public int? CostCenter { get; set; }

        public string Activity { get; set; }

        public bool HoursLoadedSuccesfully { get; set; }

        public string Title { get; set; }

        public decimal HoursApproved { get; set; }

        public decimal AllHoursMustLoad { get; set; }
    }

    public class WorkTimeReportModel
    {
        public IList<WorkTimeReportModelItem> Items { get; set; }

        public IList<EmployeeAllocationResume> EmployeesAllocationResume { get; set; }

        public bool IsCompleted { get; set; }

        public bool WorkTimeReportByHours { get; set; }

        public IList<EmployeeMissingHours> EmployeesMissingHours { get; set; }
    }

    public class EmployeeAllocationResume
    {
        public int EmployeeId { get; set; }

        public string Employee { get; set; }

        public string LastMonthDescription { get; set; }

        public int LastMonth { get; set; }

        public string CurrentMonthDescription { get; set; }

        public int CurrentMonth { get; set; }

        public decimal LastPercentage { get; set; }

        public decimal CurrentPercentage { get; set; }

        public bool MissAnyPercentageAllocation
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(LastMonthDescription) && !string.IsNullOrWhiteSpace(CurrentMonthDescription))
                {
                    return (LastPercentage + CurrentPercentage) != 200;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(LastMonthDescription))
                    {
                        return CurrentPercentage != 100;
                    }
                    else
                    {
                        return LastPercentage != 100;
                    }
                }
            }
        }
    }

    public class EmployeeToRecalculate
    {
        public int EmployeeId { get; set; }

        public int Count { get; set; }

        public decimal Percentage { get; set; }
    }

    public class EmployeeMissingHours
    {
        public string EmployeeNumber { get; set; }

        public string Name { get; set; }

        public string Manager { get; set; }

        public decimal Hours { get; set; }

        public decimal HoursMustLoad { get; set; }

        public decimal MissingHours
        {
            get
            {
                if (Facturability > 0)
                {
                    if (WorkTimeReportByHours)
                    {
                        return HoursMustLoad - Hours;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public decimal Facturability { get; set; }

        public bool WorkTimeReportByHours { get; set; }
    }
}
