using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.Rrhh;
using Sofco.Domain.Rh.Tiger;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Repository.Rh.Repositories.Interfaces;

namespace Sofco.Service.Implementations.Rrhh
{
    public class RrhhService : IRrhhService
    {
        private readonly IWorktimeData worktimeData;
        private readonly ILogMailer<RrhhService> logger;
        private readonly ITigerEmployeeRepository tigerEmployeeRepository;
        private readonly IUnitOfWork unitOfWork;

        private readonly string contributionsName = "Aportes y Contribuciones total";
        private readonly int contributionsNumber = 648001;

        public RrhhService(IWorktimeData worktimeData, ILogMailer<RrhhService> logger, ITigerEmployeeRepository tigerEmployeeRepository, IUnitOfWork unitOfWork)
        {
            this.worktimeData = worktimeData;
            this.logger = logger;
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
        }

        public Response<byte[]> GenerateTigerTxt(bool allUsers)
        {
            var response = new Response<byte[]>();

            var items = worktimeData.GetAllTigerReport().OrderBy(x => x.EmployeeNumber);

            if (!items.Any())
            {
                response.AddError(Resources.Rrhh.TigerReport.NotFound);
                return response;
            }

            if (!allUsers)
            {
                try
                {
                    var item = items.FirstOrDefault();
                    var employee = unitOfWork.EmployeeRepository.GetByEmployeeNumber(item.EmployeeNumber);
                    employee.ExcludeForTigerReport = true;

                    unitOfWork.EmployeeRepository.Update(employee);
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                }
            }

            var memoryStream = new MemoryStream();
            var text = new StreamWriter(memoryStream);

            try
            {
                foreach (var tigerReportItem in items)
                {
                    text.WriteLine(tigerReportItem.GetLine());
                    text.Flush();

                    var allocation = new Allocation
                    {
                        Id = tigerReportItem.AllocationId,
                        RealPercentage = Convert.ToDecimal(tigerReportItem.Percentage)
                    };

                    unitOfWork.AllocationRepository.UpdateRealPercentage(allocation);
                }

                try
                {
                    unitOfWork.Save();
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                }

                response.Data = memoryStream.GetBuffer();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }

        public Response UpdateSocialCharges(int year, int month)
        {
            var response = new Response();

            var today = DateTime.UtcNow;

            if (!DatesHelper.ValidateMonth(month))
                response.AddError(Resources.Rrhh.Prepaid.MonthError);

            if (year < today.AddYears(-1).Year || year > today.Year)
                response.AddError(Resources.Rrhh.Prepaid.YearError);

            if (response.HasErrors()) return response;
            
            var socialChargesData = tigerEmployeeRepository.GetSocialCharges(year, month);
            var employees = unitOfWork.EmployeeRepository.GetIdAndEmployeeNumber(year, month);

            var listToUpdate = new List<SocialCharge>();
            var listToAdd = new List<SocialCharge>();

            if (socialChargesData.Any())
            {
                try
                {
                    FillData(year, month, socialChargesData, listToUpdate, listToAdd, employees);
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                    response.AddError(Resources.Common.ErrorSave);
                    return response;
                }

                foreach (var socialCharge in listToAdd)
                {
                    var salaryTotal = socialCharge.Items
                        .Where(x => x.AccountNumber == 641100)
                        .Sum(x => Convert.ToDecimal(CryptographyHelper.Decrypt(x.Value)));

                    var chargesTotal = socialCharge.Items
                        .Where(x => x.AccountNumber != 641100 && x.AccountNumber != 641300 && x.AccountNumber != 648001)
                        .Sum(x => Convert.ToDecimal(CryptographyHelper.Decrypt(x.Value)));

                    socialCharge.SalaryTotal = CryptographyHelper.Encrypt(salaryTotal.ToString(CultureInfo.InvariantCulture));
                    socialCharge.ChargesTotal = CryptographyHelper.Encrypt(chargesTotal.ToString(CultureInfo.InvariantCulture));
                }

                foreach (var socialCharge in listToUpdate)
                {
                    var salaryTotal = socialCharge.Items
                        .Where(x => x.AccountNumber == 641100)
                        .Sum(x => Convert.ToDecimal(CryptographyHelper.Decrypt(x.Value)));

                    var chargesTotal = socialCharge.Items
                        .Where(x => x.AccountNumber != 641100 && x.AccountNumber != 641300 && x.AccountNumber != 648001)
                        .Sum(x => Convert.ToDecimal(CryptographyHelper.Decrypt(x.Value)));

                    socialCharge.SalaryTotal = CryptographyHelper.Encrypt(salaryTotal.ToString(CultureInfo.InvariantCulture));
                    socialCharge.ChargesTotal = CryptographyHelper.Encrypt(chargesTotal.ToString(CultureInfo.InvariantCulture));
                }

                try
                {
                    if (listToAdd.Any()) unitOfWork.RrhhRepository.Add(listToAdd);

                    if (listToUpdate.Any()) unitOfWork.RrhhRepository.Update(listToUpdate);

                    unitOfWork.Save();

                    response.AddSuccess(Resources.Rrhh.Prepaid.SocialChargesSynced);
                }
                catch (Exception e)
                {
                    logger.LogError(e);
                    response.AddError(Resources.Common.ErrorSave);
                }
            }
            else
            {
                response.AddWarning(Resources.Common.SearchEmpty);
            }

            return response;
        }

        public Response UpdateManagers()
        {
            var response = new Response();

            var firstDayMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

            try
            {
                var employees = unitOfWork.RrhhRepository.GetEmployeesWithBestAllocation(firstDayMonth);

                foreach (var employee in employees)
                {
                    if(unitOfWork.GroupRepository.IsManagerOrDirector(employee)) continue;

                    var firstAllocation = employee.Allocations.FirstOrDefault();

                    if (firstAllocation != null && firstAllocation.Analytic.ManagerId.HasValue && employee.ManagerId != firstAllocation.Analytic.ManagerId)
                    {
                        unitOfWork.EmployeeRepository.UpdateManager(employee.Id, firstAllocation.Analytic.ManagerId.Value);
                    }
                }

                response.AddSuccess(Resources.Common.SaveSuccess);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.GeneralError);
            }

            return response;
        }

        public Response<IList<SalaryReportItem>> GetSalaryReport(DateTime startDate, DateTime endDate)
        {
            var reponse = new Response<IList<SalaryReportItem>>();

            return reponse;
        }

        private void FillData(int year, int month, IList<EmployeeSocialCharges> socialChargesData, List<SocialCharge> listToUpdate, List<SocialCharge> listToAdd, IList<Tuple<int, string, string>> employees)
        {
            var gapsSocialCharge = unitOfWork.RrhhRepository.GetSocialCharges(year, month);
            var listEmployeeContributions = new List<string>();

            foreach (var data in socialChargesData)
            {
                var employeeSocialCharge = gapsSocialCharge.FirstOrDefault(x => x.Employee.EmployeeNumber == data.EmployeeNumber);

                if (employeeSocialCharge != null)
                {
                    var item = employeeSocialCharge.Items.FirstOrDefault(x => x.AccountNumber == data.AccountNumber);

                    if (item != null)
                    {
                        var valueToCompare = Convert.ToDecimal(CryptographyHelper.Decrypt(item.Value));

                        if (valueToCompare != data.Value)
                        {
                            item.Value = CryptographyHelper.Encrypt(data.Value.ToString(CultureInfo.InvariantCulture));
                            listToUpdate.Add(employeeSocialCharge);
                        }

                        if (!listEmployeeContributions.Contains(data.EmployeeNumber))
                        {
                            var contributionAccountToCompare = employeeSocialCharge.Items.FirstOrDefault(x => x.AccountNumber == contributionsNumber);

                            if (contributionAccountToCompare != null)
                            {
                                var employee = employees.FirstOrDefault(x => x.Item2 == data.EmployeeNumber);

                                if (employee != null)
                                {
                                    listEmployeeContributions.Add(data.EmployeeNumber);

                                    var contributionValueToCompare = Convert.ToDecimal(CryptographyHelper.Decrypt(employee.Item3));

                                    if (contributionValueToCompare != Convert.ToDecimal(CryptographyHelper.Decrypt(contributionAccountToCompare.Value)))
                                    {
                                        contributionAccountToCompare.Value = CryptographyHelper.Encrypt(employee.Item3.ToString(CultureInfo.InvariantCulture));

                                        if (listToUpdate.All(x => x.Id != employeeSocialCharge.Id))
                                        {
                                            listToUpdate.Add(employeeSocialCharge);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var employee = employees.FirstOrDefault(x => x.Item2 == data.EmployeeNumber);

                                if (employee != null)
                                {
                                    employeeSocialCharge.Items.Add(new SocialChargeItem
                                    {
                                        Value = employee.Item3,
                                        AccountName = contributionsName,
                                        AccountNumber = contributionsNumber
                                    });

                                    listToUpdate.Add(employeeSocialCharge);
                                    listEmployeeContributions.Add(data.EmployeeNumber);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (employeeSocialCharge.Items == null) employeeSocialCharge.Items = new List<SocialChargeItem>();

                        employeeSocialCharge.Items.Add(new SocialChargeItem
                        {
                            Value = CryptographyHelper.Encrypt(data.Value.ToString(CultureInfo.InvariantCulture)),
                            AccountName = data.AccountName,
                            AccountNumber = data.AccountNumber
                        });

                        listToUpdate.Add(employeeSocialCharge);
                    }
                }
                else
                {
                    var employee = employees.FirstOrDefault(x => x.Item2 == data.EmployeeNumber);

                    if (employee == null) continue;

                    var socialToAdd = listToAdd.FirstOrDefault(x => x.EmployeeId == employee.Item1);

                    if (socialToAdd != null)
                    {
                        socialToAdd.Items.Add(new SocialChargeItem
                        {
                            Value = CryptographyHelper.Encrypt(data.Value.ToString(CultureInfo.InvariantCulture)),
                            AccountName = data.AccountName,
                            AccountNumber = data.AccountNumber
                        });
                    }
                    else
                    {
                        var itemToAdd = new SocialCharge
                        {
                            Year = year,
                            Month = month,
                            EmployeeId = employee.Item1,
                            Items = new List<SocialChargeItem>()
                            {
                                new SocialChargeItem
                                {
                                    Value = CryptographyHelper.Encrypt(data.Value.ToString(CultureInfo.InvariantCulture)),
                                    AccountName = data.AccountName,
                                    AccountNumber = data.AccountNumber
                                }
                            }
                        };

                        itemToAdd.Items.Add(new SocialChargeItem
                        {
                            Value = employee.Item3,
                            AccountName = contributionsName,
                            AccountNumber = contributionsNumber
                        });

                        listToAdd.Add(itemToAdd);
                    }
                }
            }
        }
    }
}
