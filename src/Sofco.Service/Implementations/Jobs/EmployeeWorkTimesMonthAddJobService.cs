using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Internal;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Jobs;
using Sofco.Domain;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Models.WorkTimeManagement;
using Sofco.Framework.Helpers;
using Sofco.Service.Implementations.Entities;

namespace Sofco.Service.Implementations.Jobs
{
    public class EmployeeWorkTimesMonthAddJobService : EmployeeWorkTimesAddJobServiceBase, IEmployeeWorkTimesMonthAddJobService
    {
        private readonly IUnitOfWork unitOfWork;

        #region private properties

        private const string DATENOTVALIDMESSAGE = "La precarga de horas mensual solamente podrá ejecutarse el dia";

        private IList<CloseDate> closesDate;
        private IList<CloseDate> ClosesDate
        {
            get
            {
                if (closesDate == null || closesDate.Count == 0)
                    closesDate = this.unitOfWork.CloseDateRepository.GetBeforeAndCurrent();

                return closesDate;
            }

        }

        #endregion



        public EmployeeWorkTimesMonthAddJobService(IUnitOfWork unitOfWork, ILogMailer<EmployeeWorkTimesAddJobService> logger) : base(unitOfWork, logger)
        {
            this.unitOfWork = unitOfWork;
        }

        protected override List<DateTime> GetDaysToAnalize()
        {
            List<DateTime> days = GetDaysBeetweenLastsCloses();
            return this.FilterDaysHolidays(days);
        }
        protected override bool MustExecute()
        {
            CloseDate closeDate = ClosesDate.First();
            CloseDate nextCloseDate = this.unitOfWork.CloseDateRepository.GetNext().FirstOrDefault();
            if (nextCloseDate != null) 
                 notExecuteMessage = String.Format("{3} {0}-{1}-{2}", nextCloseDate.Day, nextCloseDate.Month, nextCloseDate.Year, DATENOTVALIDMESSAGE);

            return closeDate.Year == DateTime.Now.Year && closeDate.Month == DateTime.Now.Month && closeDate.Day == DateTime.Now.Day;
        }
        private List<DateTime> GetDaysBeetweenLastsCloses()
        {
            CloseDate currentCloseDate = ClosesDate.First();
            CloseDate previousCloseDate = ClosesDate.Skip(1).First();
            DateTime date = new DateTime(currentCloseDate.Year, currentCloseDate.Month, currentCloseDate.Day);
            DateTime previousDate = new DateTime(previousCloseDate.Year, previousCloseDate.Month, previousCloseDate.Day);

            List<DateTime> days = new List<DateTime>();
            while (date.Date != previousDate.Date)
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    days.Add(date);
                }
                date = date.AddDays(-1);
            }
            days.Reverse();
            return days;
        }

      
    }
}
