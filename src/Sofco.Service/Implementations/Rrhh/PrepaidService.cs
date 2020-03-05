using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models.Rrhh;
using Sofco.Core.Services.ManagementReport;
using Sofco.Core.Services.Rrhh;
using Sofco.Domain.Enums;
using Sofco.Domain.Utils;
using Sofco.Framework.Helpers;
using Sofco.Framework.MailData;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Sofco.Domain.Models.Rrhh;

namespace Sofco.Service.Implementations.Rrhh
{
    public class PrepaidService : IPrepaidService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILogMailer<PrepaidService> logger;
        private readonly IPrepaidFactory prepaidFactory;
        private readonly IMailSender mailSender;
        private readonly EmailConfig emailConfig;
        private readonly IManagementReportCalculateCostsService managementReportCalculateCostsService;

        private readonly string contributionsName = "Prepagas (por asiento)";
        private readonly int contributionsNumber = 648000;
        private readonly int costoNetoNumber = 962;

        public PrepaidService(IUnitOfWork unitOfWork,
            ILogMailer<PrepaidService> logger,
            IPrepaidFactory prepaidFactory,
            IOptions<EmailConfig> emailOptions,
            IManagementReportCalculateCostsService managementReportCalculateCostsService,
            IMailSender mailSender)
        {
            this.unitOfWork = unitOfWork;
            this.logger = logger;
            this.prepaidFactory = prepaidFactory;
            this.mailSender = mailSender;
            this.emailConfig = emailOptions.Value;
            this.managementReportCalculateCostsService = managementReportCalculateCostsService;
        }

        public Response<PrepaidDashboard> Import(int prepaidId, int yearId, int monthId, IFormFile file)
        {
            var response = new Response<PrepaidDashboard>();

            var prepaid = unitOfWork.UtilsRepository.GetPrepaid(prepaidId);

            if (prepaid == null)
                response.AddError(Resources.Rrhh.Prepaid.NotFound);

            if (monthId < 1 || monthId > 12)
                response.AddError(Resources.Rrhh.Prepaid.MonthError);

            var today = DateTime.UtcNow;

            if (yearId < today.AddYears(-1).Year || yearId > today.Year)
                response.AddError(Resources.Rrhh.Prepaid.YearError);

            if (response.HasErrors()) return response;

            if (unitOfWork.PrepaidImportedDataRepository.DateIsClosed(prepaidId, yearId, monthId))
            {
                response.AddError(Resources.Rrhh.Prepaid.DateClosed);
                return response;
            }

            try
            {
                var fileManager = prepaidFactory.GetInstance(prepaid?.Code);

                if (fileManager == null)
                {
                    response.AddError(Resources.Rrhh.Prepaid.NotImplemented);
                    return response;
                }

                unitOfWork.PrepaidImportedDataRepository.DeleteByDateAndPrepaid(prepaid.Id, new DateTime(yearId, monthId, 1));

                response = fileManager.Process(yearId, monthId, file, prepaid);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.SaveFileError);
            }

            return response;
        }

        public Response<PrepaidResponse> GetDashboard(int yearId, int monthId)
        {
            var response = new Response<PrepaidResponse>() { Data = new PrepaidResponse() };

            response.Data.Dashboard = unitOfWork.PrepaidImportedDataRepository.GetDashboard(yearId, monthId);
            response.Data.MustSyncWithTiger = !unitOfWork.RrhhRepository.ExistData(yearId, monthId);

            return response;
        }

        public Response<PrepaidImportedDataModel> Get(int yearId, int monthId)
        {
            var response = new Response<PrepaidImportedDataModel> { Data = new PrepaidImportedDataModel() };

            response.Data.Items = unitOfWork.PrepaidImportedDataRepository.GetByDate(yearId, monthId);

            response.Data.Totals = new List<PrepaidTotals>();

            var prepaids = unitOfWork.UtilsRepository.GetPrepaids();

            if (prepaids.Any())
            {
                var first = prepaids.FirstOrDefault();

                if(first != null)
                    response.Data.IsClosed = unitOfWork.PrepaidImportedDataRepository.DateIsClosed(first.Id, yearId, monthId);
            }
            
            foreach (var prepaid in prepaids)
            {
                response.Data.Totals.Add(new PrepaidTotals
                {
                    Prepaid = prepaid.Text,
                    PrepaidValue = response.Data.Items.Where(x => x.PrepaidId == prepaid.Id).Sum(x => x.PrepaidCost),
                    TigerValue = response.Data.Items.Where(x => x.PrepaidId == prepaid.Id).Sum(x => x.TigerCost),
                });
            }

            response.Data.Provisioneds = unitOfWork.PrepaidImportedDataRepository.GetLastProvisioneds(new DateTime(yearId, monthId, 1));

            return response;
        }

        public Response Update(PrepaidImportedDataUpdateModel model)
        {
            var response = new Response();

            if (model.Ids == null || !model.Ids.Any())
            {
                response.AddError(Resources.Rrhh.Prepaid.NoItemsSelected);
            }

            if (!model.Status.HasValue)
            {
                response.AddError(Resources.Rrhh.Prepaid.StatusEmpty);
            }

            try
            {
                var data = unitOfWork.PrepaidImportedDataRepository.GetByIds(model.Ids);

                foreach (var prepaidImportedData in data)
                {
                    var socialcharge = unitOfWork.EmployeeRepository.GetSocialCharges(prepaidImportedData.EmployeeId, prepaidImportedData.Date);

                    if (socialcharge != null)
                    {
                        var item = socialcharge.Items.SingleOrDefault(x => x.AccountNumber == costoNetoNumber);

                        if (item != null)
                        {
                            //var encryptedValue = CryptographyHelper.Encrypt(prepaidImportedData.CostDifference.ToString(CultureInfo.InvariantCulture));

                            //if (!encryptedValue.Equals(item.Value))
                            //{
                            //    item.Value = encryptedValue;
                            //}

                            item.AccountNumber = contributionsNumber;
                        }
                        else
                        {
                            //var newItem = new SocialChargeItem
                            //{
                            //    AccountName = contributionsName,
                            //    AccountNumber = contributionsNumber,
                            //    Value = CryptographyHelper.Encrypt(prepaidImportedData.CostDifference.ToString(CultureInfo.InvariantCulture))
                            //};

                            //socialcharge.Items.Add(newItem);
                        }

                        var chargesTotal = socialcharge.Items
                            .Where(x => x.AccountNumber != 641100 && x.AccountNumber != 641101 && x.AccountNumber != 641300 && x.AccountNumber != 960 && x.AccountNumber != 962 && x.AccountNumber != 930)
                            .Sum(x => Convert.ToDecimal(CryptographyHelper.Decrypt(x.Value)));

                        socialcharge.ChargesTotal = CryptographyHelper.Encrypt(chargesTotal.ToString(CultureInfo.InvariantCulture));

                        unitOfWork.RrhhRepository.Update(socialcharge);
                    }

                    prepaidImportedData.Status = model.Status.GetValueOrDefault();
                    unitOfWork.PrepaidImportedDataRepository.UpdateStatus(prepaidImportedData);
                }

                if (data.Any())
                {
                    unitOfWork.Save();
                    response.AddSuccess(Resources.Rrhh.Prepaid.UpdateSuccess);
                }
                else
                    response.AddWarning(Resources.Rrhh.Prepaid.NoDataUpdate);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Close(int yearId, int monthId)
        {
            var response = new Response();

            var list = unitOfWork.PrepaidImportedDataRepository.GetByDate(yearId, monthId);

            if (list.Any(x => x.Status == PrepaidImportedDataStatus.Error))
            {
                response.AddError(Resources.Rrhh.Prepaid.CannotClose);
                return response;
            }

            try
            {
                foreach (var prepaidImportedData in list)
                {
                    prepaidImportedData.Closed = true;
                    unitOfWork.PrepaidImportedDataRepository.Close(prepaidImportedData);
                }

                unitOfWork.Save();
                response.AddSuccess(Resources.Rrhh.Prepaid.CloseSuccess);

                SendMailToDaf(response, yearId, monthId);

                managementReportCalculateCostsService.UpdateManagementReports(response, yearId, monthId);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response InformToRrhhPrepaidsImported(int yearId, int monthId)
        {
            var response = new Response();

            try
            {
                var prepaidImportedDataIds = unitOfWork.PrepaidImportedDataRepository.GetEmployeeIds(yearId, monthId);

                var employeeMissingInFiles = unitOfWork.EmployeeRepository.GetMissingEmployess(prepaidImportedDataIds);

                foreach (var employee in employeeMissingInFiles)
                {
                    if (!decimal.TryParse(CryptographyHelper.Decrypt(employee.PrepaidAmount), out var prepaidAmount)) prepaidAmount = 0;

                    var itemToAdd = new PrepaidImportedData
                    {
                        Date = new DateTime(yearId, monthId, 1).Date,
                        Cuil = employee.Cuil.ToString(),
                        Dni = employee.DocumentNumber.ToString(),
                        EmployeeId = employee.Id,
                        EmployeeName = employee.Name,
                        EmployeeNumber = employee.EmployeeNumber,
                        Period = new DateTime(yearId, monthId, 1).Date,
                        Status = PrepaidImportedDataStatus.Error,
                        TigerBeneficiaries = employee.BeneficiariesCount,
                        TigerCost = prepaidAmount,
                        TigerPlan = employee.PrepaidPlan,
                        Comments = Resources.Rrhh.Prepaid.EmployeeMissing
                    };

                    unitOfWork.PrepaidImportedDataRepository.Insert(itemToAdd);
                }

                if (employeeMissingInFiles.Any())
                {
                    unitOfWork.Save();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }

            try
            {
                var rrhhGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.RrhhCode);

                var data = new MailDefaultData()
                {
                    Title = Resources.Mails.MailSubjectResource.PrepaidImported,
                    Message = string.Format(Resources.Mails.MailMessageResource.PrepaidImported, DatesHelper.GetDateDescription(new DateTime(yearId, monthId, 1))),
                    Recipients = new List<string> { rrhhGroup.Email }
                };

                mailSender.Send(data);
                response.AddSuccess(Resources.Common.MailSent);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSendMail);
            }

            return response;
        }

        private void SendMailToDaf(Response response, int yearId, int monthId)
        {
            try
            {
                var dafGroup = unitOfWork.GroupRepository.GetByCode(emailConfig.DafCode);

                var data = new MailDefaultData()
                {
                    Title = Resources.Mails.MailSubjectResource.PrepaidClosed,
                    Message = string.Format(Resources.Mails.MailMessageResource.PrepaidClosed, DatesHelper.GetDateDescription(new DateTime(yearId, monthId, 1))),
                    Recipients = new List<string> { dafGroup.Email }
                };

                mailSender.Send(data);
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ErrorSendMail);
            }
        }
    }
}
