﻿using System;
using Sofco.Core.Services.AllocationManagement;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Logger;
using Sofco.Core.Mail;
using Sofco.Framework.StatusHandlers.Analytic;
using Sofco.Model.Utils;
using Sofco.Framework.ValidationHelpers.AllocationManagement;
using Sofco.Model.Enums;
using Sofco.Model.Models.AllocationManagement;

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

        private const string MailSubject = "Apertura analítica {0}";

        private const string MailBody = "<font size='3'>" +
                                            "<span style='font-size:12pt'>" +
                                                "Estimados, </br></br>" +
                                                "Se dio de alta la analítica <strong>{0}</strong>, puede acceder a la misma desde el siguiente <a href='{1}' target='_blank'>link</a></br></br>" +
                                                "Saludos" +
                                            "</span>" +
                                        "</font>";

        public AnalyticService(IUnitOfWork unitOfWork, IMailSender mailSender, ILogMailer<AnalyticService> logger, 
            IOptions<CrmConfig> crmOptions, IOptions<EmailConfig> emailOptions, IMailBuilder mailBuilder)
        {
            this.unitOfWork = unitOfWork;
            this.mailSender = mailSender;
            this.logger = logger;
            crmConfig = crmOptions.Value;
            this.emailConfig = emailOptions.Value;
            this.mailBuilder = mailBuilder;
        }

        public ICollection<Analytic> GetAllActives()
        {
            return unitOfWork.AnalyticRepository.GetAllOpenReadOnly();
        }

        public ICollection<Analytic> GetAll()
        {
            return unitOfWork.AnalyticRepository.GetAllReadOnly();
        }

        public AnalyticOptions GetOptions()
        {
            var options = new AnalyticOptions();

            options.Activities = unitOfWork.UtilsRepository.GetImputationNumbers().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Directors = unitOfWork.UserRepository.GetDirectors().Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();
            options.Sellers = unitOfWork.UserRepository.GetSellers(this.emailConfig.SellerCode).Select(x => new Option { Id = x.Id, Text = x.Name }).ToList();
            options.Managers = unitOfWork.UserRepository.GetManagers().Select(x => new ListItem<string> { Id = x.Id, Text = x.Name, ExtraValue = x.ExternalManagerId}).ToList();
            options.Currencies = unitOfWork.UtilsRepository.GetCurrencies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Solutions = unitOfWork.UtilsRepository.GetSolutions().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Technologies = unitOfWork.UtilsRepository.GetTechnologies().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.Products = unitOfWork.UtilsRepository.GetProducts().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.ClientGroups = unitOfWork.UtilsRepository.GetClientGroups().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
            options.PurchaseOrders = unitOfWork.UtilsRepository.GetPurchaseOrders().Select(x => new Option { Id = x.Id, Text = x.Text }).ToList();
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

        public Response<IList<Allocation>> GetResources(int id)
        {
            var response = new Response<IList<Allocation>>();

            var resources = unitOfWork.AnalyticRepository.GetResources(id);

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
            AnalyticValidationHelper.CheckNameAndDescription(response, analytic);
            AnalyticValidationHelper.CheckDirector(response, analytic);
            AnalyticValidationHelper.CheckCurrency(response, analytic);
            AnalyticValidationHelper.CheckDates(response, analytic);

            if (response.HasErrors()) return response;

            try
            {
                unitOfWork.AnalyticRepository.Insert(analytic);
                unitOfWork.Save();

                response.AddSuccess(Resources.AllocationManagement.Analytic.SaveSuccess);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
            }

            await UpdateAnalyticOnCrm(response, analytic.Title, analytic.ServiceId);
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
            AnalyticValidationHelper.CheckNameAndDescription(response, analytic);
            AnalyticValidationHelper.CheckDirector(response, analytic);
            AnalyticValidationHelper.CheckCurrency(response, analytic);
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

        public Response Close(int analyticId)
        {
            var response = new Response();
            var analytic = AnalyticValidationHelper.Find(response, unitOfWork.AnalyticRepository, analyticId);

            if (response.HasErrors()) return response;

            try
            {
                AnalyticStatusClose.Save(analytic, unitOfWork, response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex);
                response.AddError(Resources.Common.ErrorSave);
                return response;
            }

            try
            {
                AnalyticStatusClose.SendMail(response, analytic, emailConfig, mailSender, unitOfWork, mailBuilder);
            }
            catch (Exception ex)
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
                this.logger.LogError(ex);
            }

            //todo: inhabilitar el servicio en crm

            return response;
        }

        private void SendMail(Analytic analytic, Response response)
        {
            try
            {
                var subject = string.Format(MailSubject, analytic.ClientExternalName);
                var body = string.Format(MailBody, analytic.Name, $"{emailConfig.SiteUrl}allocationManagement/analytics/{analytic.Id}");

                var mailPmo = unitOfWork.GroupRepository.GetEmail(emailConfig.PmoCode);
                var mailDaf = unitOfWork.GroupRepository.GetEmail(emailConfig.DafCode);
                var mailRrhh = unitOfWork.GroupRepository.GetEmail(emailConfig.RrhhCode);
                var director = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.DirectorId);
                var manager = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.ManagerId);
                var seller = unitOfWork.UserRepository.GetSingle(x => x.Id == analytic.CommercialManagerId);

                var recipients = $"{mailPmo};{mailDaf};{director.Email};{manager.Email};{seller.Email};{mailRrhh}";

                mailSender.Send(recipients, subject, body);
            }
            catch (Exception ex)
            {
                response.AddWarning(Resources.Common.ErrorSendMail);
                this.logger.LogError(ex);
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
    }
}
