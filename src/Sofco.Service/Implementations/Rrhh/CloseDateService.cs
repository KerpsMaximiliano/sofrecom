using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Common;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.Rrhh
{
    public class CloseDateService : ICloseDateService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<CloseDateService> logger;
        //private readonly ICloseDateApproverManager closeDateApproverManager;

        public CloseDateService(IUnitOfWork unitOfWork, ILogMailer<CloseDateService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response Add(IList<CloseDate> model)
        {
            var response = new Response();

            if (!model.All(x => x.Day >= 1 && x.Day <= 28))
            {
                response.AddError(Resources.Rrhh.CloseDate.DayError);
                return response;
            }

            try
            {
                foreach (var item in model)
                {
                    var today = DateTime.Now.Date;

                    if (item.Year < today.Year)
                        continue;
                    else 
                        if (item.Year == today.Year && item.Month < today.Month) continue;

                    if (item.Id == 0)
                        unitOfWork.CloseDateRepository.Insert(item);
                    else
                        unitOfWork.CloseDateRepository.Update(item);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Rrhh.CloseDate.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<CloseDateModel> Get(int startMonth, int startYear, int endMonth, int endYear)
        {
            var response = new Response<CloseDateModel> { Data = new CloseDateModel { Items = new List<CloseDateModelItem>() } };

            var closeDates = unitOfWork.CloseDateRepository.Get(startMonth, startYear, endMonth, endYear);
            var settingCloseMonth = unitOfWork.SettingRepository.GetByKey(SettingConstant.CloseMonthKey);
            var closeMonthValue = Convert.ToInt32(settingCloseMonth.Value);

            var startDate = new DateTime(startYear, startMonth, 1);
            var endDate = new DateTime(endYear, endMonth, 1);

            response.Data.CloseMonthValue = closeMonthValue;

            while (startDate.Date <= endDate.Date)
            {
                var domain = closeDates.SingleOrDefault(x => x.Year == startDate.Year && x.Month == startDate.Month);
                var item = new CloseDateModelItem();

                if (domain != null)
                {
                    item.Id = domain.Id;
                    item.Day = domain.Day;
                    item.Month = domain.Month;
                    item.Year = domain.Year;
                    item.Description = DatesHelper.GetDateDescription(new DateTime(domain.Year, domain.Month, 1));
                }
                else
                {
                    item.Id = 0;
                    item.Day = closeMonthValue;
                    item.Month = startDate.Month;
                    item.Year = startDate.Year;
                    item.Description = DatesHelper.GetDateDescription(new DateTime(startDate.Year, startDate.Month, 1));
                }

                response.Data.Items.Add(item);

                startDate = startDate.AddMonths(1);
            }

            return response;
        }

        public IList<CloseDate> GetFirstBeforeNextMonth()
        {
            var closeDates = this.unitOfWork.CloseDateRepository.GetFirstBeforeNextMonth();

            return closeDates;
        }
    }
}
