using System;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Utils;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Sofco.Framework.Helpers;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<ContractService> logger;

        public ContractService(IUnitOfWork unitOfWork, ILogMailer<ContractService> logger)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
        }

        public Response<IList<AccountInfoModel>> GetAccountsInfo(int year, int month)
        {
            var response = new Response<IList<AccountInfoModel>> { Data = new List<AccountInfoModel>() };

            if (month < 1 || month > 12)
                response.AddError(Resources.Rrhh.Prepaid.MonthError);

            var today = DateTime.UtcNow;

            if (year < today.AddYears(-5).Year || year > today.Year)
                response.AddError(Resources.Rrhh.Prepaid.YearError);

            try
            {
                var socialCharges = unitOfWork.RrhhRepository.GetSocialCharges(year, month);

                var employeeIds = socialCharges.Select(x => x.EmployeeId).ToList();

                var allocations = unitOfWork.AllocationRepository.GetByEmployeesAndDate(employeeIds, year, month);

                foreach (var allocation in allocations)
                {
                    var socialCharge = socialCharges.SingleOrDefault(x => x.EmployeeId == allocation.EmployeeId);

                    if(allocation.RealPercentage <= 0) continue;

                    foreach (var socialChargeItem in socialCharge.Items)
                    {
                        var itemToAdd = new AccountInfoModel();

                        itemToAdd.Year = year;
                        itemToAdd.Month = month;
                        itemToAdd.Account = socialChargeItem.AccountName;
                        itemToAdd.AccountNumber = socialChargeItem.AccountNumber;
                        itemToAdd.Analytic = allocation.Analytic?.Title;
                        itemToAdd.Employee = socialCharge.Employee?.Name;
                        itemToAdd.EmployeeNumber = socialCharge.Employee?.EmployeeNumber;
                        itemToAdd.Percentage = allocation.RealPercentage;

                        if (decimal.TryParse(CryptographyHelper.Decrypt(socialChargeItem.Value), out var value))
                        {
                            itemToAdd.Ammount = itemToAdd.Percentage * value / 100;
                        }

                        response.Data.Add(itemToAdd);
                    }
                }

                if (response.Data.Count > 0)
                {
                    response.Data = response.Data.OrderBy(x => x.Employee).ThenBy(x => x.AccountNumber).ToList();
                }
                else
                {
                    response.AddWarning(Resources.Common.SearchEmpty);
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }
    }
}
