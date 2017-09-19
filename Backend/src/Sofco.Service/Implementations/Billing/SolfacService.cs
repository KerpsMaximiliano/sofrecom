using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.Mail;
using Sofco.Framework.StatusHandlers.Solfac;
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
        private readonly IUserRepository _userRepository;

        public SolfacService(ISolfacRepository solfacRepository, 
            IInvoiceRepository invoiceRepository, 
            IHostingEnvironment hostingEnvironment, 
            IUserRepository userRepository)
        {
            _solfacRepository = solfacRepository;
            _invoiceRepository = invoiceRepository;
            _hostingEnvironment = hostingEnvironment;
            _userRepository = userRepository;
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

                _solfacRepository.Save();

                response.Data = solfac;
                response.Messages.Add(new Message(Resources.es.Billing.Solfac.SolfacCreated, MessageType.Success));
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

        public Response ChangeStatus(int solfacId, SolfacStatus status, EmailConfig emailConfig, int userId, string comment)
        {
            var response = new Response();

            var solfac = _solfacRepository.GetByIdWithUser(solfacId);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            return ChangeStatus(solfac, status, emailConfig, userId, comment);
        }

        public Response ChangeStatus(Solfac solfac, SolfacStatus status, EmailConfig emailConfig, int userId, string comment)
        {
            var response = new Response();

            try
            {
                var solfacStatusHandler = SolfacStatusFactory.GetInstance(status);

                // Validate status
                var statusErrors = solfacStatusHandler.Validate(solfac, comment);

                if (statusErrors.HasErrors())
                {
                    response.AddMessages(statusErrors.Messages);
                    return response;
                }
              
                // Update Status
                var solfacToModif = new Solfac { Id = solfac.Id, Status = status };
                _solfacRepository.UpdateStatus(solfacToModif);

                // Add history
                var history = GetHistory(solfac.Id, solfac.Status, status, userId, comment);
                _solfacRepository.AddHistory(history);

                // Save
                _solfacRepository.Save();
                response.Messages.Add(new Message(solfacStatusHandler.GetSuccessMessage(), MessageType.Success));

                // Send Mail
                HandleSendMail(emailConfig, solfacStatusHandler, solfac);
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var solfac = _solfacRepository.GetSingle(x => x.Id == id);

            if (solfac == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            try
            {
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
    }
}
