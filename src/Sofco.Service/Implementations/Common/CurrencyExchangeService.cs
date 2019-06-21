using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using Sofco.Common.Settings;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Common;
using Sofco.Core.Services.Common;
using Sofco.Domain.Models.Common;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.Common
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogMailer<CurrencyExchangeService> logger;

        private readonly AppSetting appSetting;

        public CurrencyExchangeService(IUnitOfWork unitOfWork, ILogMailer<CurrencyExchangeService> logger, IOptions<AppSetting> appSettingOptions)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.appSetting = appSettingOptions.Value;
        }

        public Response<int> Add(CurrencyExchangeAddModel model)
        {
            var response = new Response<int>();

            Validate(model, response);

            if (response.HasErrors()) return response;

            try
            { 
                var domain = new CurrencyExchange
                {
                    Date = new DateTime(model.Year.GetValueOrDefault(), model.Month.GetValueOrDefault(), 1).Date,
                    CurrencyId = model.CurrencyId.GetValueOrDefault(),
                    Exchange = model.Exchange.GetValueOrDefault()
                };

                unitOfWork.CurrencyExchangeRepository.Insert(domain);
                unitOfWork.Save();

                response.Data = domain.Id;

                response.AddSuccess(Resources.ManagementReport.CurrencyExchange.AddSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Update(int id, CurrencyExchangeUpdateModel model)
        {
            var response = new Response();

            if (!model.Exchange.HasValue || model.Exchange.Value <= 0 || model.Exchange.Value > 999) response.AddError(Resources.ManagementReport.CurrencyExchange.ExchangeRequired);

            var domain = unitOfWork.CurrencyExchangeRepository.Get(id);

            if (domain == null) response.AddError(Resources.ManagementReport.CurrencyExchange.NotFound);

            if (response.HasErrors()) return response;

            try
            {
                domain.Exchange = model.Exchange.GetValueOrDefault();

                unitOfWork.CurrencyExchangeRepository.Update(domain);
                unitOfWork.Save();

                response.AddSuccess(Resources.ManagementReport.CurrencyExchange.UpdateSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<CurrencyExchangeModel>> Get(int startMonth, int startYear, int endMonth, int endYear)
        {
            var response = new Response<IList<CurrencyExchangeModel>> { Data = new List<CurrencyExchangeModel>() };

            var startDate = new DateTime(startYear, startMonth, 1);
            var endDate = new DateTime(endYear, endMonth, 1);

            var currencies = unitOfWork.UtilsRepository.GetCurrencies().Where(x => x.Id != appSetting.CurrencyPesos).ToList();
            var currencyExchanges = unitOfWork.CurrencyExchangeRepository.Get(startDate, endDate);

            while (startDate.Date <= endDate.Date)
            {
                var domains = currencyExchanges.Where(x => x.Date.Date == startDate.Date).ToList();

                var itemToAdd = new CurrencyExchangeModel
                {
                    Year = startDate.Year,
                    Month = startDate.Month,
                    Description = DatesHelper.GetDateDescription(startDate),
                    Items = new List<CurrencyExchangeItemModel>()
                };

                if (domains.Any())
                {
                    foreach (var currency in currencies)
                    {
                        var curr = domains.SingleOrDefault(x => x.CurrencyId == currency.Id);

                        if (curr != null)
                            itemToAdd.Items.Add(new CurrencyExchangeItemModel { CurrencyId = currency.Id, CurrencyDesc = currency.Text, Exchange = curr.Exchange, Id = curr.Id });
                        else
                            itemToAdd.Items.Add(new CurrencyExchangeItemModel { CurrencyId = currency.Id, CurrencyDesc = currency.Text, Exchange = 0, Id = 0 });
                    }
                }
                else
                {
                    foreach (var currency in currencies)
                    {
                        itemToAdd.Items.Add(new CurrencyExchangeItemModel { CurrencyId = currency.Id, CurrencyDesc = currency.Text, Exchange = 0, Id = 0 });
                    }
                }

                response.Data.Add(itemToAdd);
                startDate = startDate.AddMonths(1);
            }

            return response;
        }

        private void Validate(CurrencyExchangeAddModel model, Response response)
        {
            if (!model.Year.HasValue || model.Year <= 0)
                response.AddError(Resources.ManagementReport.CurrencyExchange.DateRequired);

            if (!model.Month.HasValue || model.Month <= 0)
                response.AddError(Resources.ManagementReport.CurrencyExchange.DateRequired);

            if (!model.CurrencyId.HasValue || !unitOfWork.UtilsRepository.ExistCurrency(model.CurrencyId.Value))
                response.AddError(Resources.ManagementReport.CurrencyExchange.CurrencyRequired);

            if (!model.Exchange.HasValue || model.Exchange.Value <= 0 || model.Exchange.Value > 999)
                response.AddError(Resources.ManagementReport.CurrencyExchange.ExchangeRequired);
        }
    }
}
