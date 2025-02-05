﻿using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Data.Admin;
using Sofco.Core.Data.AllocationManagement;
using Sofco.Core.Data.Billing;
using Sofco.Core.FileManager;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Core.Managers;
using Sofco.Core.Models;
using Sofco.Core.Models.AllocationManagement;
using Sofco.Core.Models.Billing;
using Sofco.Core.Services.AllocationManagement;
using Sofco.Domain.Enums;
using Sofco.Domain.Models.AllocationManagement;
using Sofco.Domain.Models.ManagementReport;
using Sofco.Domain.Utils;
using Sofco.Framework.MailData;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Resources.Mails;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sofco.Service.Implementations.AllocationManagement
{
    public class AnalyticService : IAnalyticService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMailSender mailSender;
        private readonly ILogMailer<AnalyticService> logger;
        private readonly EmailConfig emailConfig;
        private readonly IMailBuilder mailBuilder;
        private readonly IEmployeeData employeeData;
        private readonly IAnalyticFileManager analyticFileManager;
        private readonly IAnalyticManager analyticManager;
        private readonly IUserData userData;
        private readonly IServiceData serviceData;
        private readonly ICustomerData customerData;
        private readonly IRoleManager roleManager;
        private readonly IAnalyticCloseManager analyticCloseManager;
        private readonly IAnalyticReopenManager analyticReopenManager;

        public AnalyticService(IUnitOfWork unitOfWork, IMailSender mailSender, ILogMailer<AnalyticService> logger,
            IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder, IServiceData serviceData,
            IEmployeeData employeeData, IAnalyticFileManager analyticFileManager,
            IUserData userData, IAnalyticManager analyticManager, IRoleManager roleManager,
            IAnalyticCloseManager analyticCloseManager, ICustomerData customerData,IAnalyticReopenManager analyticReopenManager)
        {
            this.unitOfWork = unitOfWork;
            this.mailSender = mailSender;
            this.logger = logger;
            emailConfig = emailOptions.Value;
            this.mailBuilder = mailBuilder;
            this.employeeData = employeeData;
            this.analyticFileManager = analyticFileManager;
            this.userData = userData;
            this.analyticManager = analyticManager;
            this.roleManager = roleManager;
            this.analyticCloseManager = analyticCloseManager;
            this.serviceData = serviceData;
            this.customerData = customerData;
            this.analyticReopenManager = analyticReopenManager;
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

            return list.Select(x => new Option { Id = x.Id, Text = $"{x.EmployeeNumber}-{x.Name}" }).ToList();
        }

        public Response<List<Option>> GetByCurrentUserRequestNote()
        {   
            var userId = userData.GetCurrentUser().Id;
            var result = unitOfWork.AnalyticRepository.GetAnalyticsRequestNote(userId).ToList();
            var response = new Response<List<Option>> { Data = new List<Option>() };

            foreach (var analytic in result)
            {
                if (response.Data.All(x => x.Id != analytic.Id))
                {
                    response.Data.Add(new Option { Id = analytic.Id, Text = $"{analytic.Title} - {analytic.Name}" });
                }
            }

            response.Data.OrderBy(x => x.Text);

            return response;
        }

        public Response<List<Option>> GetByCurrentUser()
        {
            var employeeId = employeeData.GetCurrentEmployee().Id;
            var userId = userData.GetCurrentUser().Id;

            var closeDates = unitOfWork.CloseDateRepository.GetBeforeCurrentAndNext();

            // Item 1: DateFrom
            // Item 2: DateTo
            var period = closeDates.GetPeriodExcludeDays();

            var result = unitOfWork.AnalyticRepository.GetAnalyticsLiteByEmployee(employeeId, userId, period.Item1.Date, period.Item2.Date).ToList();

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

            var analyticsByManagers = roleManager.HasFullAccess()
                ? unitOfWork.AnalyticRepository.GetAll()
                : unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);

            var result = analyticsByManagers.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();

            return new Response<List<Option>> { Data = result };
        }

        public Response<List<Option>> GetByLoggedManagerId()
        {
            List<Option> options = new List<Option>();
            if (roleManager.IsPmo() || roleManager.IsRrhh())
            {
                options = this.GetAllActives().Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).OrderBy(y => y.Text).ToList();
            } 
            else
            {
                var currentUser = userData.GetCurrentUser();
                var analyticsByManagers = unitOfWork.AnalyticRepository.GetAnalyticsByManagerId(currentUser.Id);
                options = analyticsByManagers.Select(x => new Option { Id = x.Id, Text = $"{x.Title} - {x.Name}" }).ToList();
            }

            return new Response<List<Option>> { Data = options };
        }

        public Response<Analytic> GetByTitle(string title)
        {
            var response = new Response<Analytic>();

            var analytic = unitOfWork.AnalyticRepository.GetByTitle(title);

            if (analytic == null)
            {
                response.AddError(Resources.AllocationManagement.Analytic.NotFound);
                return response;
            }

            response.Data = analytic;
            return response;
        }

        public Response<List<AnalyticSearchViewModel>> Get(AnalyticSearchParameters searchParameters)
        {
            var response = new Response<List<AnalyticSearchViewModel>> { Data = new List<AnalyticSearchViewModel>() };

            if (roleManager.IsPmo() || roleManager.IsRrhh() || roleManager.IsCdg() || roleManager.IsCompliance() || roleManager.IsDafOrGaf())
            {
                var analytics = unitOfWork.AnalyticRepository.GetBySearchCriteria(searchParameters);

                response.Data = Translate(analytics);
            }
            else
            {
                if (roleManager.IsManager() || roleManager.IsDirector() || roleManager.IsManagementReportDelegate())
                {
                    var analytics = unitOfWork.AnalyticRepository.GetBySearchCriteria(searchParameters);

                    var currentUser = userData.GetCurrentUser();
                    var analyticsByDelegates = unitOfWork.DelegationRepository.GetByGrantedUserIdAndType(currentUser.Id, DelegationType.ManagementReport);

                    foreach (var analytic in analytics)
                    {
                        if (analytic.ManagerId.GetValueOrDefault() == currentUser.Id ||
                            analytic.Sector?.ResponsableUserId == currentUser.Id ||
                            analyticsByDelegates.Any(x => x.AnalyticSourceId == analytic.Id))
                        {
                            response.Data.Add(new AnalyticSearchViewModel(analytic));
                        }
                    }
                }
            }

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

        public Response<IList<OpportunityOption>> GetOpportunities(int id)
        {
            var response = new Response<IList<OpportunityOption>> { Data = new List<OpportunityOption>() };

            var analytic = unitOfWork.AnalyticRepository.Get(id);

            if (!string.IsNullOrWhiteSpace(analytic?.ServiceId))
            {
                var projects = unitOfWork.ProjectRepository.GetAllActives(analytic.ServiceId);

                response.Data = projects.Select(x => new OpportunityOption
                {
                    Id = $"{x.OpportunityNumber}",
                    ProjectId = x.Id.ToString(),
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
            options.Managers = unitOfWork.UserRepository.GetManagers().Distinct().Select(x => new ListItem<string> { Id = x.Id, Text = x.Name, ExtraValue = x.ExternalManagerId }).ToList();
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

            response.Data = AnalyticValidationHelper.Find(response, unitOfWork, id);

            return response;
        }

        public Response<Analytic> GetByIdWithOnlyPendingRefunds(int id)
        {
            var response = new Response<Analytic>();

            response.Data = AnalyticValidationHelper.Find(response, unitOfWork, id, false);

            return response;
        }

        public Response<IList<Allocation>> GetTimelineResources(int id, DateTime dateSince, int months)
        {
            var response = new Response<IList<Allocation>>();

            var startDate = new DateTime(dateSince.Year, dateSince.Month, 1);
            var endDateAux = dateSince.AddMonths(months - 1);
            var endDate = new DateTime(endDateAux.Year, endDateAux.Month, DateTime.DaysInMonth(endDateAux.Year, endDateAux.Month));

            var resources = unitOfWork.AnalyticRepository.GetTimelineResources(id, startDate, endDate);

            if (!resources.Any())
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.ResourcesNotFound, MessageType.Warning));
            }

            response.Data = resources;

            return response;
        }

        public Response<Analytic> Add(Analytic analytic)
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
                    var manager = unitOfWork.UserRepository.Get(analytic.ManagerId.GetValueOrDefault());

                    var service = unitOfWork.ServiceRepository.GetByIdCrm(analytic.ServiceId);

                    if (service != null)
                    {
                        service.Analytic = analytic.Title;

                        if (manager != null)
                        {
                            service.ManagerId = manager.ExternalManagerId;
                            service.Manager = manager.Name;
                        }

                        unitOfWork.ServiceRepository.UpdateAnalyticAndManager(service);

                        customerData.ClearKeys(manager != null ? manager.UserName : "*");
                        serviceData.ClearKeys(manager != null ? manager.UserName : "*", service.AccountId);
                    }
                }

                CreateManagementReport(analytic);

                if (analytic.SolutionId == 0) analytic.SolutionId = null;
                if (analytic.TechnologyId == 0) analytic.TechnologyId = null;
                if (analytic.ServiceTypeId == 0) analytic.ServiceTypeId = null;

                unitOfWork.AnalyticRepository.Insert(analytic);

                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Analytic.SaveSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            if (!string.IsNullOrWhiteSpace(analytic.ServiceId))
            {
                var crmResponse = analyticManager.UpdateCrmAnalytic(analytic);

                if (crmResponse.HasErrors())
                {
                    response.AddMessages(crmResponse.Messages);
                }
            }

            SendMail(analytic, response);

            return response;
        }

        private void CreateManagementReport(Analytic analytic)
        {
            try
            {
                analytic.ManagementReport = new Domain.Models.ManagementReport.ManagementReport
                {
                    Analytic = analytic,
                    StartDate = analytic.StartDateContract.Date,
                    Status = ManagementReportStatus.CdgPending,
                    EndDate = analytic.EndDateContract.Date
                };

                if (string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    analytic.ManagementReport.Budgets = new List<Budget>();
                    analytic.ManagementReport.CostDetails = new List<CostDetail>();

                    for (var date = new DateTime(analytic.StartDateContract.Year, analytic.StartDateContract.Month, 1).Date;
                        date.Date <= analytic.EndDateContract.Date;
                        date = date.AddMonths(1))
                    {
                        analytic.ManagementReport.CostDetails.Add(new CostDetail
                        {
                            ManagementReport = analytic.ManagementReport,
                            MonthYear = date.Date
                        });
                    }
                }
                else
                {
                    analytic.ManagementReport.Billings = new List<ManagementReportBilling>();
                    analytic.ManagementReport.CostDetails = new List<CostDetail>();

                    for (var date = new DateTime(analytic.StartDateContract.Year, analytic.StartDateContract.Month, 1).Date;
                        date.Date <= analytic.EndDateContract.Date;
                        date = date.AddMonths(1))
                    {
                        analytic.ManagementReport.Billings.Add(new ManagementReportBilling
                        {
                            ManagementReport = analytic.ManagementReport,
                            MonthYear = date.Date
                        });

                        analytic.ManagementReport.CostDetails.Add(new CostDetail
                        {
                            ManagementReport = analytic.ManagementReport,
                            MonthYear = date.Date
                        });
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogError(e);
            }
        }

        public Response<NewTitleModel> GetNewTitle(int costCenterId)
        {
            var response = new Response<NewTitleModel> { Data = new NewTitleModel() };

            if (costCenterId == 0)
            {
                response.AddError(Resources.AllocationManagement.Analytic.CostCenterIsRequired);
                return response;
            }

            var analytic = unitOfWork.AnalyticRepository.GetLastAnalytic(costCenterId);

            if (analytic == null)
            {
                var costCenter = unitOfWork.CostCenterRepository.GetSingle(x => x.Id == costCenterId);
                response.Data.Title = $"{costCenter.Code}-{costCenter.Letter}0001";
                response.Data.TitleId = 1;
            }
            else
            {
                analytic.TitleId++;
                response.Data.Title = $"{analytic.CostCenter.Code}-{analytic.CostCenter.Letter}{analytic.TitleId.ToString().PadLeft(4, '0')}";
                response.Data.TitleId = analytic.TitleId;
            }

            return response;
        }

        public Response<Analytic> Update(AnalyticModel analyticModel)
        {
            var response = new Response<Analytic>();

            var analytic = AnalyticValidationHelper.Find(response, unitOfWork, analyticModel.Id);

            analyticModel.UpdateDomain(analytic);

            AnalyticValidationHelper.CheckName(response, analytic);
            AnalyticValidationHelper.CheckDirector(response, analytic);
            AnalyticValidationHelper.CheckDates(response, analytic);

            if (response.HasErrors()) return response;

            try
            {
                var manager = unitOfWork.UserRepository.Get(analytic.ManagerId.GetValueOrDefault());

                var service = unitOfWork.ServiceRepository.GetByIdCrm(analytic.ServiceId);

                if (service != null && manager != null)
                {
                    service.ManagerId = manager.ExternalManagerId;
                    service.Manager = manager.Name;

                    unitOfWork.ServiceRepository.UpdateAnalyticAndManager(service);

                    customerData.ClearKeys();
                    serviceData.ClearKeys();
                }

                unitOfWork.AnalyticRepository.Update(analytic);
                unitOfWork.Save();

                if (!string.IsNullOrWhiteSpace(analytic.AccountId) &&
                    !string.IsNullOrWhiteSpace(analytic.ServiceId))
                {
                    var crmResponse = analyticManager.UpdateCrmAnalytic(analytic);
                    if (crmResponse.HasErrors())
                    {
                        response.AddMessages(crmResponse.Messages);
                    }
                }

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
            return analyticCloseManager.Close(analyticId, status);
        }
        public Response Reopen(int analyticId, AnalyticStatus status)
        {
            return analyticReopenManager.Reopen(analyticId, status);
        }

        private void SendMail(Analytic analytic, Response response)
        {
            try
            {
                var subject = string.Format(MailSubjectResource.AddAnalytic, analytic.AccountName);
                var body = string.Format(MailMessageResource.AddAnalytic, $"{analytic.Title} - {analytic.Name}", $"{emailConfig.SiteUrl}contracts/analytics/{analytic.Id}/view");

                //var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode); // 2023-08-17
                var mailGaf = unitOfWork.GroupRepository.GetEmail(emailConfig.GafCode); // 2023-08-17
                var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);
                //var mailCompliance = unitOfWork.GroupRepository.GetEmail(emailConfig.ComplianceCode);
                //var mailQuality = unitOfWork.GroupRepository.GetEmail(emailConfig.QualityCode); // 2023-08-17
                //var mailCdg = unitOfWork.GroupRepository.GetEmail(emailConfig.CdgCode);

                var recipientsList = new List<string>();

                //recipientsList.AddRange(new[] { mailPmo, mailRrhh, mailGaf, mailCompliance, mailQuality, mailCdg });
                recipientsList.AddRange(new[] {
                    //mailPmo, // 2023-08-17
                    mailGaf,
                    "mscovello@sofredigital.com.ar",
                    "fgiani@sofredigital.com.ar",
                    "dacaruso@sofredigital.com.ar", // 2023-08-17
                    mailRrhh,
                    //mailQuality // 2023-08-17
                });

                var manager = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.ManagerId);
                var seller = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.CommercialManagerId);
                var sector = unitOfWork.SectorRepository.Get(analytic.SectorId);
                var director = unitOfWork.UserRepository.Get(sector.ResponsableUserId);

                if (manager != null) recipientsList.Add(manager.Email);
                if (seller != null) recipientsList.Add(seller.Email);
                if (director != null) recipientsList.Add(director.Email);

                var data = new AddAnalyticData
                {
                    Title = subject,
                    Message = body,
                    Recipients = recipientsList.Distinct().ToList()
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

        private List<AnalyticSearchViewModel> Translate(List<Analytic> data)
        {
            return data.Select(x => new AnalyticSearchViewModel(x)).ToList();
        }
        public Response<Analytic> GetByIdWhitRefund(int id)
        {
            var response = new Response<Analytic>();
            var result = unitOfWork.AnalyticRepository.GetById(id);
            if (result == null)
            {
                response.Messages.Add(new Message(Resources.AllocationManagement.Analytic.ResourcesNotFound, MessageType.Warning));
            }
            
            response.Data = result;

            return response;
        }
    }
}
