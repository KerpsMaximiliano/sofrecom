using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Sofco.Core.Config;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.Mail;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class SolfacService : ISolfacService
    {
        private readonly ISolfacRepository _solfacRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISolfacStatusFactory _solfacStatusFactory;
        private readonly CrmConfig _crmConfig;

        public SolfacService(ISolfacRepository solfacRepository, 
            IInvoiceRepository invoiceRepository,
            ISolfacStatusFactory solfacStatusFactory, 
            IHostingEnvironment hostingEnvironment,
            IOptions<CrmConfig> crmOptions)
        {
            _solfacRepository = solfacRepository;
            _invoiceRepository = invoiceRepository;
            _hostingEnvironment = hostingEnvironment;
            _solfacStatusFactory = solfacStatusFactory;
            _crmConfig = crmOptions.Value;
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
                _solfacRepository.Insert(solfac);

                // Update Invoice Status to Related
                if (solfac.InvoiceId.HasValue && solfac.InvoiceId.Value > 0)
                {
                    var invoiceToModif = new Invoice { Id = solfac.InvoiceId.Value, InvoiceStatus = InvoiceStatus.Related };
                    _invoiceRepository.UpdateStatus(invoiceToModif);
                }

                // Save
                _solfacRepository.Save();

                response.Data = solfac;
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacCreated, MessageType.Success));

                UpdateHitos(solfac.Hitos, response);
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public IList<Solfac> Search(SolfacParams parameter)
        {
            return _solfacRepository.SearchByParams(parameter);
        }

        public IList<Hito> GetHitosByProject(string projectId)
        {
            return _solfacRepository.GetHitosByProject(projectId);
        }

        public IList<Solfac> GetByProject(string projectId)
        {
            return _solfacRepository.GetByProject(projectId);
        }

        public Response<Solfac> GetById(int id)
        {
            var response = new Response<Solfac>();

            var solfac = _solfacRepository.GetById(id);

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

            var solfac = _solfacRepository.GetByIdWithUser(solfacId);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            return ChangeStatus(solfac, parameters, emailConfig);
        }

        public Response ChangeStatus(Solfac solfac, SolfacStatusParams parameters, EmailConfig emailConfig)
        {
            var response = new Response();

            var solfacStatusHandler = _solfacStatusFactory.GetInstance(parameters.Status);

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
                solfacStatusHandler.SaveStatus(solfac, parameters, _solfacRepository);

                // Add history
                var history = GetHistory(solfac.Id, solfac.Status, parameters.Status, parameters.UserId, parameters.Comment);
                _solfacRepository.AddHistory(history);

                // Save
                _solfacRepository.Save();
                response.Messages.Add(new Message(solfacStatusHandler.GetSuccessMessage(), MessageType.Success));

                // Update Hitos
                solfacStatusHandler.UpdateHitos(_solfacRepository.GetHitosIdsBySolfacId(solfac.Id), solfac, _crmConfig.Url);
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                HandleSendMail(emailConfig, solfacStatusHandler, solfac);
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSendMail, MessageType.Error));
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var solfac = _solfacRepository.GetSingle(x => x.Id == id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.NotFound, MessageType.Error));
                return response;
            }


            try
            {
                if (solfac.InvoiceId.HasValue)
                {
                    var invoice = _invoiceRepository.GetSingle(x => x.Id == solfac.InvoiceId);
                    invoice.InvoiceStatus = InvoiceStatus.Approved;
                    _invoiceRepository.UpdateStatus(invoice);
                }

                _solfacRepository.Delete(solfac);
                _solfacRepository.Save();

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.Deleted, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<SolfacHistory> GetHistories(int id)
        {
            return _solfacRepository.GetHistories(id);
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
                _solfacRepository.Update(solfac);

                // Update Invoice Status to Related
                if (solfac.InvoiceId.HasValue && solfac.InvoiceId.Value > 0)
                {
                    var invoiceToModif = new Invoice { Id = solfac.InvoiceId.Value, InvoiceStatus = InvoiceStatus.Related };
                    _invoiceRepository.UpdateStatus(invoiceToModif);
                }

                // Save changes
                _solfacRepository.Save();

                UpdateHitos(solfac.Hitos, response);

                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacUpdated, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<SolfacAttachment> SaveFile(int solfacId, byte[] fileAsArrayBytes, string fileFileName)
        {
            var response = new Response<SolfacAttachment>();

            var solfac = _solfacRepository.GetSingle(x => x.Id == solfacId);

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

            _solfacRepository.SaveAttachment(attachment);
            _solfacRepository.Save();

            response.Data = attachment;
            response.Messages.Add(new Message(Resources.es.Billing.Solfac.FileAdded, MessageType.Success));

            return response;
        }

        public ICollection<SolfacAttachment> GetFiles(int solfacId)
        {
            return _solfacRepository.GetFiles(solfacId);
        }

        public Response<SolfacAttachment> GetFileById(int fileId)
        {
            var response = new Response<SolfacAttachment>();

            var file = _solfacRepository.GetFileById(fileId);

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

            var file = _solfacRepository.GetFileById(id);

            if (file == null)
            {
                response.Messages.Add(new Message(Resources.es.Common.FileNotFound, MessageType.Error));
                return response;
            }

            try
            {
                _solfacRepository.DeleteFile(file);
                _solfacRepository.Save();
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.FileDeleted, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        private Response<Solfac> Validate(Solfac solfac)
        {
            var response = new Response<Solfac>();

            if (solfac.OtherProvince1Percentage > 0 && solfac.Province1Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince2Percentage > 0 && solfac.Province2Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            if (solfac.OtherProvince3Percentage > 0 && solfac.Province3Id == 0)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.ProvinceRequired, MessageType.Error));

            foreach (var hito in solfac.Hitos)
            {
                if (hito.Quantity <= 0)
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.HitoQuantityRequired, MessageType.Error));

                if (hito.UnitPrice <= 0)
                    response.Messages.Add(new Message(Resources.es.Billing.Solfac.HitoUnitPriceRequired, MessageType.Error));
            }

            var totalPercentage = solfac.BuenosAiresPercentage + solfac.CapitalPercentage +
                                  solfac.OtherProvince1Percentage + solfac.OtherProvince2Percentage +
                                  solfac.OtherProvince3Percentage;

            if (totalPercentage != 100)
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.TotalPercentageError, MessageType.Error));

            if (solfac.CapitalPercentage < 0 || solfac.BuenosAiresPercentage < 0 ||
                solfac.OtherProvince1Percentage < 0 || solfac.OtherProvince2Percentage < 0 ||
                solfac.OtherProvince3Percentage < 0)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.PercentageLessThan0, MessageType.Error));
            }

            if (solfac.TimeLimit <= 0)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.TimeLimitLessThan0, MessageType.Error));
            }

            return response;
        }

        private void HandleSendMail(EmailConfig emailConfig, ISolfacStatusHandler solfacStatusHandler, Solfac solfac)
        {
            if (!_hostingEnvironment.IsStaging() && !_hostingEnvironment.IsProduction()) return;

            var subject = solfacStatusHandler.GetSubjectMail(solfac);
            var body = solfacStatusHandler.GetBodyMail(solfac, emailConfig.SiteUrl);
            var recipients = solfacStatusHandler.GetRecipients(solfac, emailConfig);

            MailSender.Send(recipients, emailConfig.EmailFrom, emailConfig.DisplyNameFrom,
                subject, body, emailConfig.SmtpServer, emailConfig.SmtpPort, emailConfig.SmtpDomain);
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
                client.BaseAddress = new Uri(_crmConfig.Url);

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
    }
}
