﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Core.DAL.Admin;
using Sofco.Framework.ValidationHandlers.Billing;
using Sofco.Core.Mail;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacService : ISolfacService
    {
        private readonly ISolfacRepository solfacRepository;
        private readonly IInvoiceRepository invoiceRepository;
        private readonly ISolfacStatusFactory solfacStatusFactory;
        private readonly IUserRepository userRepository;
        private readonly CrmConfig crmConfig;
        private readonly IMailSender mailSender;

        public SolfacService(ISolfacRepository solfacRepository, 
            IInvoiceRepository invoiceRepository,
            ISolfacStatusFactory solfacStatusFactory, 
            IUserRepository userRepository,
            IOptions<CrmConfig> crmOptions,
            IMailSender mailSender)
        {
            this.solfacRepository = solfacRepository;
            this.invoiceRepository = invoiceRepository;
            this.solfacStatusFactory = solfacStatusFactory;
            this.crmConfig = crmOptions.Value;
            this.userRepository = userRepository;
            this.mailSender = mailSender;
        }

        public Response<Solfac> Add(Solfac solfac)
        {
            var response = Validate(solfac);

            if (response.HasErrors()) return response;

            try
            {
                solfac.UpdatedDate = DateTime.Now;
                solfac.ModifiedByUserId = solfac.UserApplicantId;

                // Add History
                solfac.Histories.Add(GetHistory(SolfacStatus.None, solfac.Status, solfac.UserApplicantId, string.Empty));

                // Insert Solfac
                solfacRepository.Insert(solfac);

                // Update Invoice Status to Related
                if (solfac.InvoiceId.HasValue && solfac.InvoiceId.Value > 0)
                {
                    var invoiceToModif = new Invoice { Id = solfac.InvoiceId.Value, InvoiceStatus = InvoiceStatus.Related };
                    invoiceRepository.UpdateStatus(invoiceToModif);
                }

                // Save
                solfacRepository.Save();

                response.Data = solfac;
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacCreated, MessageType.Success));

                UpdateHitos(solfac.Hitos, response);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Solfac> Search(SolfacParams parameter, string userMail)
        {
            var isDirector = userRepository.HasDirectorGroup(userMail);

            if (isDirector)
            {
                return solfacRepository.SearchByParams(parameter);
            }
            else
            {
                return solfacRepository.SearchByParamsAndUser(parameter, userMail);
            }
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return solfacRepository.GetHitosByProject(projectId);
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return solfacRepository.GetByProject(projectId);
        }

        public Response<Solfac> GetById(int id)
        {
            var response = new Response<Solfac>();

            var solfac = solfacRepository.GetById(id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            response.Data = solfac;
            return response;
        }

        public Response ChangeStatus(int solfacId, SolfacStatusParams parameters, EmailConfig emailConfig)
        {
            var response = new Response<SolfacChangeStatusResponse>();

            var solfac = SolfacValidationHelper.ValidateIfExistAndGetWithUser(solfacId, _solfacRepository, response);

            if (response.HasErrors()) return response;

            return ChangeStatus(solfac, parameters, emailConfig);
        }

        public Response ChangeStatus(Solfac solfac, SolfacStatusParams parameters, EmailConfig emailConfig)
        {
            var response = new Response();

            var solfacStatusHandler = solfacStatusFactory.GetInstance(parameters.Status);

            try
            {
                // Validate status
                var statusErrors = solfacStatusHandler.Validate(solfac, parameters);

                if (statusErrors.HasErrors())
                {
                    response.AddMessages(statusErrors.Messages);
                    return response;
                }

                // Update Status
                solfacStatusHandler.SaveStatus(solfac, parameters, solfacRepository);

                // Add history
                var history = GetHistory(solfac.Id, solfac.Status, parameters.Status, parameters.UserId, parameters.Comment);
                solfacRepository.AddHistory(history);

                // Save
                solfacRepository.Save();
                response.Messages.Add(new Message(solfacStatusHandler.GetSuccessMessage(), MessageType.Success));

                // Update Hitos
                solfacStatusHandler.UpdateHitos(solfacRepository.GetHitosIdsBySolfacId(solfac.Id), solfac, crmConfig.Url);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                HandleSendMail(emailConfig, solfacStatusHandler, solfac);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSendMail, MessageType.Error));
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, _solfacRepository, response);

            if (response.HasErrors()) return response;

            try
            {
                if (solfac.InvoiceId.HasValue)
                {
                    var invoice = invoiceRepository.GetSingle(x => x.Id == solfac.InvoiceId);
                    invoice.InvoiceStatus = InvoiceStatus.Approved;
                    invoiceRepository.UpdateStatus(invoice);
                }

                solfacRepository.Delete(solfac);
                solfacRepository.Save();

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.Deleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<SolfacHistory> GetHistories(int id)
        {
            return solfacRepository.GetHistories(id);
        }

        public Response Update(Solfac solfac, string comments)
        {
            var response = Validate(solfac);

            if (response.HasErrors()) return response;

            try
            {
                solfac.UpdatedDate = DateTime.Now;

                // Add History
                solfac.Histories.Add(GetHistory(solfac.Status, solfac.Status, solfac.UserApplicantId, comments));

                // Insert Solfac
                solfacRepository.Update(solfac);

                // Update Invoice Status to Related
                if (solfac.InvoiceId.HasValue && solfac.InvoiceId.Value > 0)
                {
                    var invoiceToModif = new Invoice { Id = solfac.InvoiceId.Value, InvoiceStatus = InvoiceStatus.Related };
                    invoiceRepository.UpdateStatus(invoiceToModif);
                }

                // Save changes
                solfacRepository.Save();

                UpdateHitos(solfac.Hitos, response);

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacUpdated, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName)
        {
            var response = new Response<SolfacAttachment>();

            var solfac = solfacRepository.GetSingle(x => x.Id == solfacId);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }

            var attachment = new SolfacAttachment
            {
                SolfacId = solfacId,
                Name = fileFileName,
                CreationDate = DateTime.Now,
                File = fileAsArrayBytes
            };

            solfacRepository.SaveAttachment(attachment);
            solfacRepository.Save();

            response.Data = attachment;
            response.Messages.Add(new Message(Resources.es.Billing.Solfac.FileAdded, MessageType.Success));

            return response;
        }

        public ICollection<SolfacAttachment> GetFiles(int solfacId)
        {
            return solfacRepository.GetFiles(solfacId);
        }

        public Response<SolfacAttachment> GetFileById(int fileId)
        {
            var response = new Response<SolfacAttachment>();

            var file = solfacRepository.GetFileById(fileId);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.es.Common.FileNotFound, MessageType.Error));
                return response;
            }

            response.Data = file;
            return response;
        }

        public Response DeleteFile(int id)
        {
            var response = new Response();

            var file = solfacRepository.GetFileById(id);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.es.Common.FileNotFound, MessageType.Error));
                return response;
            }

            try
            {
                solfacRepository.DeleteFile(file);
                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.FileDeleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        private Response<Solfac> Validate(Solfac solfac)
        {
            var response = new Response<Solfac>();

            SolfacValidationHelper.ValidateProvincePercentage(solfac, response);
            SolfacValidationHelper.ValidateHitos(solfac.Hitos, response);
            SolfacValidationHelper.ValidateHitos(solfac.Hitos, response);
            SolfacValidationHelper.ValidatePercentage(solfac, response);
            SolfacValidationHelper.ValidateTimeLimit(solfac, response);

            return response;
        }

        private void HandleSendMail(EmailConfig emailConfig, ISolfacStatusHandler solfacStatusHandler, Solfac solfac)
        {
            var subject = solfacStatusHandler.GetSubjectMail(solfac);
            var body = solfacStatusHandler.GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipients = solfacStatusHandler.GetRecipients(solfac, emailConfig);

            mailSender.Send(recipients, subject, body);
        }

        private SolfacHistory GetHistory(int solfacId, SolfacStatus statusFrom, SolfacStatus statusTo, int userId, string comment)
        {
            var history = GetHistory(statusFrom, statusTo, userId, comment);
            history.SolfacId = solfacId;
            return history;
        }

        private SolfacHistory GetHistory(SolfacStatus statusFrom, SolfacStatus statusTo, int userId, string comment)
        {
            var history = new SolfacHistory
            {
                SolfacStatusFrom = statusFrom,
                SolfacStatusTo = statusTo,
                UserId = userId,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            return history;
        }

        private async void UpdateHitos(IList<Hito> hitos, Response response)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(crmConfig.Url);

                foreach (var item in hitos)
                {
                    try
                    {
                        var description = string.Empty;
                        if (item.Description != item.DescriptionOld)
                        {
                            description = $"Name={item.Description}";
                        }

                        var unitPrice = string.Empty;
                        if (item.UnitPrice != item.UnitPriceOld)
                        {
                            unitPrice = $"Ammount={item.UnitPrice}";
                        }

                        var stringContent = new StringContent(string.Join("&", description, unitPrice), Encoding.UTF8, "application/x-www-form-urlencoded");
                        var httpResponse = await client.PutAsync($"/api/InvoiceMilestone/{item.ExternalHitoId}", stringContent);

                        httpResponse.EnsureSuccessStatusCode();
                    }
                    catch (Exception)
                    {
                        response.Messages.Add(new Message(Resources.es.Billing.Solfac.ErrorSaveOnHitos, MessageType.Warning));
                    }
                }
            }
        }

        public Response UpdateBill(int id, SolfacStatusParams parameters)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExistAndGetWithUser(id, _solfacRepository, response);

            if (response.HasErrors()) return response;

            SolfacValidationHelper.ValidateInvoiceCode(parameters, _solfacRepository, response);
            SolfacValidationHelper.ValidateInvoiceDate(parameters, response);

            if (response.HasErrors()) return response;

            try
            {
                var solfacToModif = new Solfac { Id = solfac.Id, InvoiceCode = parameters.InvoiceCode, InvoiceDate = parameters.InvoiceDate };
                solfacRepository.UpdateInvoice(solfacToModif);

                var history = GetHistory(solfac.Id, solfac.Status, solfac.Status, parameters.UserId, string.Empty);
                solfacRepository.AddHistory(history);

                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.BillUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response UpdateCashedDate(int id, SolfacStatusParams parameters)
        {
            var response = new Response();

            var solfac = SolfacValidationHelper.ValidateIfExist(id, _solfacRepository, response);

            if (response.HasErrors()) return response;

            SolfacValidationHelper.ValidateCasheDate(parameters, response);

            if (response.HasErrors()) return response;

            try
            {
                var solfacToModif = new Solfac { Id = solfac.Id, CashedDate = parameters.CashedDate };
                solfacRepository.UpdateCash(solfacToModif);

                var history = GetHistory(solfac.Id, solfac.Status, solfac.Status, parameters.UserId, string.Empty);
                solfacRepository.AddHistory(history);

                solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.CashUpdated, MessageType.Success));
            }
            catch (Exception ex)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }
    }
}
