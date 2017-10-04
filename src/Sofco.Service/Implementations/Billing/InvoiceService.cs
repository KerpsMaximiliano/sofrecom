using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Framework.Mail;
using Sofco.Framework.StatusHandlers.Invoice;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IInvoiceStatusFactory _invoiceStatusFactory;

        public InvoiceService(IInvoiceRepository invoiceRepository, 
            IUserRepository userRepository, 
            IHostingEnvironment hostingEnvironment,
            IInvoiceStatusFactory invoiceStatusFactory)
        {
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
            _hostingEnvironment = hostingEnvironment;
            _invoiceStatusFactory = invoiceStatusFactory;
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            return _invoiceRepository.GetByProject(projectId);
        }

        public Response<Invoice> GetById(int id)
        {
            var response = new Response<Invoice>();

            var invoce = _invoiceRepository.GetById(id);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;

            return response;
        }

        public Response<Invoice> Add(Invoice invoice, string identityName)
        {
            var response = new Response<Invoice>();

            try
            {
                var userName = identityName.Split('@')[0];
                var user = _userRepository.GetSingle(x => x.UserName == userName);

                if (user == null)
                {
                    response.Messages.Add(new Message(Resources.es.Admin.User.NotFound, MessageType.Error));
                    return response;
                }

                invoice.InvoiceStatus = InvoiceStatus.SendPending;
                invoice.InvoiceNumber = "0000-00000000";
                invoice.UserId = user.Id;

                // Add History
                invoice.Histories.Add(GetHistory(InvoiceStatus.None, invoice.InvoiceStatus, invoice.UserId, string.Empty));

                // Insert Solfac
                _invoiceRepository.Insert(invoice);

                // Save
                _invoiceRepository.Save();

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.InvoiceCreated, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> SaveExcel(Invoice invoice)
        {
            var response = new Response<Invoice>();

            try
            {
                var datetime = DateTime.Now;

                var invoiceToSave = new Invoice
                {
                    Id = invoice.Id,
                    ExcelFile = invoice.ExcelFile,
                    ExcelFileName = string.Concat("REMITO_", invoice.AccountName, "_", invoice.Service, "_", invoice.Project, "_", datetime.ToString("yyyyMMdd"), ".xlsx"),
                    ExcelFileCreatedDate = datetime
                };

                _invoiceRepository.UpdateExcel(invoiceToSave);
                _invoiceRepository.Save();

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.ExcelUpload, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> GetExcel(int invoiceId)
        {
            var response = new Response<Invoice>();

            var invoce = _invoiceRepository.GetExcel(invoiceId);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;
            return response;
        }

        public Response<Invoice> SavePdf(Invoice invoice)
        {
            var response = new Response<Invoice>();

            try
            {
                var datetime = DateTime.Now;

                var invoiceToSave = new Invoice
                {
                    Id = invoice.Id,
                    PdfFile = invoice.PdfFile,
                    PdfFileName = string.Concat("REMITO_", invoice.AccountName, "_", invoice.Service, "_", invoice.Project, "_", datetime.ToString("yyyyMMdd"), ".pdf"),
                    PdfFileCreatedDate = datetime
                };

                _invoiceRepository.UpdatePdf(invoiceToSave);
                _invoiceRepository.Save();

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.PdfUpload, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> GetPdf(int invoiceId)
        {
            var response = new Response<Invoice>();

            var invoce = _invoiceRepository.GetPdf(invoiceId);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;
            return response;
        }

        public Response ChangeStatus(int invoiceId, InvoiceStatus status, EmailConfig emailConfig, InvoiceStatusParams parameters)
        {
            var response = new Response();

            var invoice = _invoiceRepository.GetById(invoiceId);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceStatusHandler = _invoiceStatusFactory.GetInstance(status);

            try
            {
                // Validate status
                var statusErrors = invoiceStatusHandler.Validate(invoice, parameters);

                if (statusErrors.HasErrors())
                {
                    response.AddMessages(statusErrors.Messages);
                    return response;
                }

                // Update Status
                invoiceStatusHandler.SaveStatus(invoice, parameters);

                // Add history
                var history = GetHistory(invoiceId, invoice.InvoiceStatus, status, parameters.UserId, parameters.Comment);
                _invoiceRepository.AddHistory(history);

                // Save
                _invoiceRepository.Save();
                response.Messages.Add(new Message(invoiceStatusHandler.GetSuccessMessage(), MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                HandleSendMail(emailConfig, invoiceStatusHandler, invoice);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSendMail, MessageType.Error));
            }

            return response;
        }

        public IList<Invoice> GetOptions(string projectId)
        {
            return _invoiceRepository.GetOptions(projectId);
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var invoice = _invoiceRepository.GetById(id);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            if (invoice.InvoiceStatus != InvoiceStatus.SendPending && invoice.InvoiceStatus != InvoiceStatus.Rejected)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.CannotDelete, MessageType.Error));
                return response;
            }

            try
            {
                _invoiceRepository.Delete(invoice);
                _invoiceRepository.Save();

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.Deleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<Invoice> Search(InvoiceParams parameters, string userMail)
        {
            var isDirector = _userRepository.HasDirectorGroup(userMail);

            if (isDirector)
            {
                return _invoiceRepository.SearchByParams(parameters);
            }
            else
            {
                return _invoiceRepository.SearchByParamsAndUser(parameters, userMail);
            }
        }

        private void HandleSendMail(EmailConfig emailConfig, IInvoiceStatusHandler invoiceStatusHandler, Invoice invoice)
        {
            if (!_hostingEnvironment.IsStaging() && !_hostingEnvironment.IsProduction()) return;

            var subject = invoiceStatusHandler.GetSubjectMail(invoice);
            var body = invoiceStatusHandler.GetBodyMail(invoice, emailConfig.SiteUrl);
            var recipients = invoiceStatusHandler.GetRecipients(invoice, emailConfig);

            MailSender.Send(recipients, emailConfig.EmailFrom, emailConfig.DisplyNameFrom,
                subject, body, emailConfig.SmtpServer, emailConfig.SmtpPort, emailConfig.SmtpDomain);
        }

        public Response<Invoice> Clone(int id)
        {
            var response = new Response<Invoice>();

            var invoice = _invoiceRepository.GetById(id);

            if(invoice == null)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceToClone = invoice.Clone();

            try
            {
                _invoiceRepository.Insert(invoiceToClone);
                _invoiceRepository.Save();
                response.Data = invoiceToClone;
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<InvoiceHistory> GetHistories(int id)
        {
            return _invoiceRepository.GetHistories(id);
        }

        private InvoiceHistory GetHistory(int invoiceId, InvoiceStatus statusFrom, InvoiceStatus statusTo, int userId, string comment)
        {
            var history = GetHistory(statusFrom, statusTo, userId, comment);
            history.InvoiceId = invoiceId;
            return history;
        }

        private InvoiceHistory GetHistory(InvoiceStatus statusFrom, InvoiceStatus statusTo, int userId, string comment)
        {
            var history = new InvoiceHistory
            {
                StatusFrom = statusFrom,
                StatusTo = statusTo,
                UserId = userId,
                Comment = comment,
                CreatedDate = DateTime.Now
            };

            return history;
        }
    }
}
