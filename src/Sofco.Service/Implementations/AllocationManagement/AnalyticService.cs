using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.CrmServices;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.Data.Billing;
using Sofco.Core.DAL;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Models;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Billing;
using Sofco.Domain;
using Sofco.Framework.MailData;
using Sofco.Framework.StatusHandlers.Analytic;
using Sofco.Domain.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Resources.Mails;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AnalyticService : IAnalyticService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailSender mailSender;
        private readonly ILogMailer<AnalyticService> logger;
        private readonly EmailConfig emailConfig;
        private readonly CrmConfig crmConfig;
        private readonly IMailBuilder mailBuilder;
        private readonly ICrmService crmService;
        private readonly IEmployeeData employeeData;
        private readonly IAnalyticFileManager analyticFileManager;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;

        public AnalyticService(IUnitOfWork unitOfWork, IMailSender mailSender, ILogMailer<AnalyticService> logger, 
            IOptions<CrmConfig> crmOptions, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder, IServiceData serviceData,
            ICrmService crmService, IEmployeeData employeeData, IAnalyticFileManager analyticFileManager, IUserData userData)
        {
            this.unitOfWork = unitOfWork;
            this.mailSender = mailSender;
            this.logger = logger;
            crmConfig = crmOptions.Value;
            emailConfig = emailOptions.Value;
            this.mailBuilder = mailBuilder;
            this.crmService = crmService;
            this.employeeData = employeeData;
            this.analyticFileManager = analyticFileManager;
            this.userData = userData;
            this.serviceData = serviceData;
        }

        public ICollection<Analytic> GetAllActives()
        {
            return unitOfWork.AnalyticRepository.GetAllOpenReadOnly();
        }

        public ICollection<AnalyticOptionForOcModel> GetByClient(string clientId, bool onlyActives)
        {
            return unitOfWork.AnalyticRepository.GetByClient(clientId, onlyActives).Select(x => new AnalyticOptionForOcModel
            {
                Id = x.Id,
                Text = $"{x.Title} - {x.Name}",
                CommercialManagerId = x.CommercialManagerId.GetValueOrDefault(),
                ManagerId = x.ManagerId.GetValueOrDefault(),
                ServiceId = x.ServiceId

            }).ToList();
        }

        public IList<Option> GetResources(int id)
        {
            var list = unitOfWork.AnalyticRepository.GetResources(id);

            return list.Select(x => new Option {Id = x.Id, Text = $"{x.EmployeeNumber}-{x.Name}"}).ToList();
        }

        public Response<List<Option>> GetByCurrentUser()
        {
            var employeeId = employeeData.GetCurrentEmployee().Id;
            var userId = userData.GetCurrentUser().Id;

            var settingCloseMonth = unitOfWork.SettingRepository.GetByKey(SettingConstant.CloseMonthKey);

            var now = DateTime.Now.Date;
            var closeMonthValue = Convert.ToInt32(settingCloseMonth.Value);

            DateTime dateFrom;
            DateTime dateTo;

            if (now.Day > closeMonthValue)
            {
                dateFrom = new DateTime(now.Year, now.Month, 1);
                dateTo = new DateTime(now.Year, now.Month + 1, 1);
            }
            else
            {
                dateFrom = new DateTime(now.Year, now.Month - 1, 1);
                dateTo = new DateTime(now.Year, now.Month, 1);
            }

            var result = unitOfWork.AnalyticRepository.GetAnalyticsLiteByEmployee(employeeId, userId, dateFrom.Date, dateTo.Date).ToList();

            var response = new Response<List<Option>> { Data = new List<Option>() };

            foreach (var analytic in result)
            {
                if (response.Data.All(x => x.Id != analytic.Id))
                {
                    response.Data.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
                }
            }

            return response;
        }

        public Response<List<Option>> GetByCurrentManager()
        {
            var currentUser = userData.GetCurrentUser();

            var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);

            var result = analyticsByManagers.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();

            return new Response<List<Option>> { Data = result };
        }

        public Response<List<AnalyticSearchViewModel>> Get(AnalyticSearchParameters searchParameters)
        {
            var response = new Response<List<AnalyticSearchViewModel>>
            {
                Data = Translate(unitOfWork.AnalyticRepository.GetBySearchCriteria(searchParameters))
            };

            return response;
        }

        public Response<byte[]> CreateReport(List<int> analytics)
        {
            var response = new Response<byte[]>();

            try
            {
                var list = unitOfWork.AnalyticRepository.GetForReport(analytics);

                var excel = analyticFileManager.CreateAnalyticReportExcel(list);

                response.Data = excel.GetAsByteArray();
            }
            catch (Exception e)
            {
                logger.LogError(e);
                response.AddError(Resources.Common.ExportFileError);
            }

            return response;
        }

        public Response UpdateDaf(Analytic analytic)
        {
            var response = new Response<Analytic>();

            AnalyticValidationHelper.Exist(response, unitOfWork.AnalyticRepository, analytic.Id);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.AnalyticRepository.UpdateDaf(analytic);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Analytic.UpdateSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response<IList<SelectListModel>> GetOpportunities(int id)
        {
            var response = new Response<IList<SelectListModel>> { Data = new List<SelectListModel>() };

            var analytic = unitOfWork.AnalyticRepository.Get(id);

            if (!string.IsNullOrWhiteSpace(analytic?.ServiceId))
            {
                var projects = unitOfWork.ProjectRepository.GetAllActives(analytic.ServiceId);

                response.Data = projects.Select(x => new SelectListModel
                {
                    Id = $"{x.OpportunityNumber} - {x.OpportunityName}",
                    Text = $"{x.OpportunityNumber} - {x.OpportunityName}"
                })
                .ToList();
            }

            return response;
        }

        public ICollection<Analytic> GetAll()
        {
            return unitOfWork.AnalyticRepository.GetAllReadOnly();
        }

        public AnalyticOptions GetOptions()
        {
            var options = new AnalyticOptions();

            options.Activities = unitOfWork.UtilsRepository.GetImputationNumbers().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Sectors = unitOfWork.UtilsRepository.GetSectors().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Sellers = unitOfWork.UserRepository.GetSellers().Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();
            options.Managers = unitOfWork.UserRepository.GetManagers().Distinct().Select(x => new ListItem<string> { Id = x.Id, Text = x.Name, ExtraValue = x.ExternalManagerId}).ToList();
            options.Currencies = unitOfWork.UtilsRepository.GetCurrencies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Solutions = unitOfWork.UtilsRepository.GetSolutions().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Technologies = unitOfWork.UtilsRepository.GetTechnologies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Products = unitOfWork.UtilsRepository.GetProducts().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.ClientGroups = unitOfWork.UtilsRepository.GetClientGroups().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.PurchaseOrders = unitOfWork.UtilsRepository.GetPurchaseOrderOptions().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.SoftwareLaws = unitOfWork.UtilsRepository.GetSoftwareLaws().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.ServiceTypes = unitOfWork.UtilsRepository.GetServiceTypes().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();

            return options;
        }

        public Response<Analytic> GetById(int id)
        {
            var response = new Response<Analytic>();

            response.Data = AnalyticValidationHelper.Find(response, unitOfWork.AnalyticRepository, id);

            return response;
        }

        public Response<IList<Allocation>> GetTimelineResources(int id, DateTime dateSince, int months)
        {
            var response = new Response<IList<Allocation>>();

            var startDate = new DateTime(dateSince.Year, dateSince.Month, 1);
            var endDateAux = dateSince.AddMonths(months-1);
            var endDate = new DateTime(endDateAux.Year, endDateAux.Month, DateTime.DaysInMonth(endDateAux.Year, endDateAux.Month));

            var resources = unitOfWork.AnalyticRepository.GetTimelineResources(id, startDate, endDate);

            if (!resources.Any())
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.ResourcesNotFound, MessageType.Warning));
            }

            response.Data = resources;

            return response;
        }

        public async Task<Response<Analytic>> Add(Analytic analytic)
        {
            var response = new Response<Analytic>();

            AnalyticValidationHelper.CheckTitle(response, analytic, unitOfWork.CostCenterRepository);
            AnalyticValidationHelper.CheckIfTitleExist(response, analytic, unitOfWork.AnalyticRepository);
            AnalyticValidationHelper.CheckName(response, analytic);
            AnalyticValidationHelper.CheckDirector(response, analytic);
            AnalyticValidationHelper.CheckDates(response, analytic);
            AnalyticValidationHelper.CheckService(response, analytic, unitOfWork.AnalyticRepository);

            if (response.HasErrors()) return response;

            try
            {

                if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    var service = unitOfWork.ServiceRepository.GetByIdCrm(analytic.ServiceId);

                    if (service != null)
                    {
                        service.Analytic = analytic.Title;
                        unitOfWork.ServiceRepository.UpdateAnalytic(service);
                    }
                }

                if (analytic.SolutionId == 0) analytic.SolutionId = null;
                if (analytic.TechnologyId == 0) analytic.TechnologyId = null;
                if (analytic.ServiceTypeId == 0) analytic.ServiceTypeId = null;

                unitOfWork.AnalyticRepository.Insert(analytic);

                unitOfWork.Save();

                serviceData.ClearKeys();

                response.AddSuccess(Resources.AllocationManagement.Analytic.SaveSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
            {
                await UpdateAnalyticOnCrm(response, analytic.Title, analytic.ServiceId);
            }
            
            SendMail(analytic, response);

            return response;
        }

        public Response<string> GetNewTitle(int costCenterId)
        {
            var response = new Response<string>();

            if (costCenterId == 0)
            {
                response.AddError(Resources.AllocationManagement.Analytic.CostCenterIsRequired);
                return response;
            }

            var analytic = unitOfWork.AnalyticRepository.GetLastAnalytic(costCenterId);

            if (analytic == null)
            {
                var costCenter = unitOfWork.CostCenterRepository.GetSingle(x => x.Id == costCenterId);
                response.Data = $"{costCenter.Code}-{costCenter.Letter}0001";
            }
            else
            {
                analytic.TitleId++;
                response.Data = $"{analytic.CostCenter.Code}-{analytic.CostCenter.Letter}{analytic.TitleId.ToString().PadLeft(4, '0')}";
            }

            return response;
        }

        public Response<Analytic> Update(Analytic analytic)
        {
            var response = new Response<Analytic>();

            AnalyticValidationHelper.Exist(response, unitOfWork.AnalyticRepository, analytic.Id);
            AnalyticValidationHelper.CheckName(response, analytic);
            AnalyticValidationHelper.CheckDirector(response, analytic);
            AnalyticValidationHelper.CheckDates(response, analytic);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.AnalyticRepository.Update(analytic);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Analytic.UpdateSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            return response;
        }

        public Response Close(int analyticId, AnalyticStatus status)
        {
            var response = new Response();
            var analytic = AnalyticValidationHelper.Find(response, unitOfWork.AnalyticRepository, analyticId);

            if (response.HasErrors()) return response;

            if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
            {
                var result = crmService.DesactivateService(new Guid(analytic.ServiceId));

                if (result.HasErrors)
                {
                    response.AddError(Resources.Common.CrmGeneralError);
                    return response;
                }
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    var service = unitOfWork.ServiceRepository.GetByIdCrm(analytic.ServiceId);

                    if (service != null)
                    {
                        service.Active = false;
                        unitOfWork.ServiceRepository.UpdateActive(service);
                    }
                }

                AnalyticStatusClose.Save(analytic, unitOfWork, response, status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);

                if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    var result = crmService.ActivateService(new Guid(analytic.ServiceId));
                    if (result.HasErrors)
                    {
                        response.AddError(Resources.Common.CrmGeneralError);
                    }
                }

                return response;
            }

            try
            {
                AnalyticStatusClose.SendMail(response, analytic, emailConfig, mailSender, unitOfWork, mailBuilder);
            }
            catch (Exception ex)
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
                logger.LogError(ex);
            }

            return response;
        }

        private void SendMail(Analytic analytic, Response response)
        {
            try
            {
                var subject = string.Format(MailSubjectResource.AddAnalytic, analytic.ClientExternalName);
                var body = string.Format(MailMessageResource.AddAnalytic, $"{analytic.Title} - {analytic.Name}", $"{emailConfig.SiteUrl}contracts/analytics/{analytic.Id}/view");

                var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);
                var mailDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
                var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);
                var mailCompliance = unitOfWork.GroupRepository.GetEmail(emailConfig.ComplianceCode);
                var mailQuality = unitOfWork.GroupRepository.GetEmail(emailConfig.QualityCode);
                var mailCdg = unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);

                var recipientsList = new List<string>();

                recipientsList.AddRange(new[] { mailPmo, mailRrhh, mailDaf, mailCompliance, mailQuality, mailCdg });

                var manager = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.ManagerId);
                var seller = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.CommercialManagerId);

                if(manager != null) recipientsList.Add(manager.Email);
                if(seller != null) recipientsList.Add(seller.Email);

                var recipients = string.Join(";", recipientsList.Distinct());

                var data = new AddAnalyticData
                {
                    Title = subject,
                    Message = body,
                    Recipients = recipients
                };

                var email = mailBuilder.GetEmail(data);

                mailSender.Send(email);
            }
            catch (Exception ex)
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
                logger.LogError(ex);
            }
        }

        private async Task UpdateAnalyticOnCrm(Response response, string analytic, string serviceId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                var data = $"Analytic={analytic}";

                var urlPath = $"/api/Service/{serviceId}/";

                try
                {
                    var stringContent = new StringContent(data, Encoding.UTF8, "application/x-www-form-urlencoded");

                    var httpResponse = await client.PutAsync(urlPath, stringContent);

                    httpResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    logger.LogError(urlPath + "; data: " + data, ex);
                    response.AddError(Resources.Common.ErrorSave);
                }
            }
        }

        private List<AnalyticSearchViewModel> Translate(List<Analytic> data)
        {
            return data.Select(x => new AnalyticSearchViewModel(x)).ToList();
        }
    }
}
