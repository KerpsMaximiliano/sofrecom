using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sofco.Core.Data.WorktimeManagement;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Services.Rrhh;
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

        public RrhhService(IWorktimeData worktimeData, ILogMailer<RrhhService> logger, ITigerEmployeeRepository tigerEmployeeRepository, IUnitOfWork unitOfWork)
        {
            this.worktimeData = worktimeData;
            this.logger = logger;
            this.tigerEmployeeRepository = tigerEmployeeRepository;
            this.unitOfWork = unitOfWork;
        }

        public Response<byte[]> GenerateTigerTxt()
        {
            var response = new Response<byte[]>();

            var items = worktimeData.GetAllTigerReport();

            if (!items.Any())
            {
                response.AddError(Resources.Rrhh.TigerReport.NotFound);
                return response;
            }

            var memoryStream = new MemoryStream();
            var text = new StreamWriter(memoryStream);

            try
            {
                foreach (var tigerReportItem in items)
                {
                    text.WriteLine(tigerReportItem.GetLine());
                    text.Flush();
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
                    socialCharge.Total = socialCharge.Items.Sum(x => x.Value);
                }

                foreach (var socialCharge in listToUpdate)
                {
                    socialCharge.Total = socialCharge.Items.Sum(x => x.Value);
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

        private void FillData(int year, int month, IList<EmployeeSocialCharges> socialChargesData, List<SocialCharge> listToUpdate, List<SocialCharge> listToAdd, IList<Tuple<int, string>> employees)
        {
            var gapsSocialCharge = unitOfWork.RrhhRepository.GetSocialCharges(year, month);

            foreach (var data in socialChargesData)
            {
                if(data.AccountNumber == 641100) continue;

                var employeeSocialCharge =
                    gapsSocialCharge.FirstOrDefault(x => x.Employee.EmployeeNumber == data.EmployeeNumber);

                if (employeeSocialCharge != null)
                {
                    var item = employeeSocialCharge.Items.FirstOrDefault(x => x.AccountNumber == data.AccountNumber);

                    if (item != null)
                    { 
                        if (item.Value != data.Value)
                        {
                            item.Value = data.Value;
                            listToUpdate.Add(employeeSocialCharge);
                        }
                    }
                    else
                    {
                        if (employeeSocialCharge.Items == null) employeeSocialCharge.Items = new List<SocialChargeItem>();

                        employeeSocialCharge.Items.Add(new SocialChargeItem
                        {
                            Value = data.Value,
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
                            Value = data.Value,
                            AccountName = data.AccountName,
                            AccountNumber = data.AccountNumber
                        });
                    }
                    else
                    {
                        listToAdd.Add(new SocialCharge
                        {
                            Year = year,
                            Month = month,
                            EmployeeId = employee.Item1,
                            Items = new List<SocialChargeItem>()
                            {
                                new SocialChargeItem
                                {
                                    Value = data.Value,
                                    AccountName = data.AccountName,
                                    AccountNumber = data.AccountNumber
                                }
                            }
                        });
                    }
                }
            }
        }
    }
}
