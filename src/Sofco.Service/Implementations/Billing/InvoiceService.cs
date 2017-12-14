using System;
using System.Collections.Generic;
using Sofco.Core.Config;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Core.Mail;

namespace Sofco.Service.Implementations.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository invoiceRepository;
        private readonly IUserRepository userRepository;
        private readonly IInvoiceStatusFactory invoiceStatusFactory;
        private readonly IMailSender mailSender;

        public InvoiceService(IInvoiceRepository invoiceRepository, 
            IUserRepository userRepository, 
            IInvoiceStatusFactory invoiceStatusFactory,
            IMailSender mailSender)
        {
            this.invoiceRepository = invoiceRepository;
            this.userRepository = userRepository;
            this.invoiceStatusFactory = invoiceStatusFactory;
            this.mailSender = mailSender;
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            return invoiceRepository.GetByProject(projectId);
        }

        public Response<Invoice> GetById(int id)
        {
            var response = new Response<Invoice>();

            var invoce = invoiceRepository.GetById(id);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
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
                var user = userRepository.GetSingle(x => x.UserName == userName);

                if (user == null)
                {
                    response.Messages.Add(new Message(Resources.Admin.User.NotFound, MessageType.Error));
                    return response;
                }

                invoice.InvoiceStatus = InvoiceStatus.SendPending;
                invoice.InvoiceNumber = "0000-00000000";
                invoice.UserId = user.Id;

                // Add History
                invoice.Histories.Add(GetHistory(InvoiceStatus.None, invoice.InvoiceStatus, invoice.UserId, string.Empty));

                // Insert Solfac
                invoiceRepository.Insert(invoice);

                // Save
                invoiceRepository.Save();

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.Billing.Invoice.InvoiceCreated, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> SaveExcel(Invoice invoice, string fileFileName)
        {
            var response = new Response<Invoice>();

            try
            {
                var datetime = DateTime.UtcNow;

                var invoiceToSave = new Invoice
                {
                    Id = invoice.Id,
                    ExcelFile = invoice.ExcelFile,
                    ExcelFileName = fileFileName,
                    ExcelFileCreatedDate = datetime
                };

                invoiceRepository.UpdateExcel(invoiceToSave);
                invoiceRepository.Save();

                invoice.ExcelFile = null;
                invoice.ExcelFileName = fileFileName;
                invoice.ExcelFileCreatedDate = datetime;

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.Billing.Invoice.ExcelUpload, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> GetExcel(int invoiceId)
        {
            var response = new Response<Invoice>();

            var invoce = invoiceRepository.GetExcel(invoiceId);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;
            return response;
        }

        public Response<Invoice> SavePdf(Invoice invoice, string fileFileName)
        {
            var response = new Response<Invoice>();

            try
            {
                var datetime = DateTime.UtcNow;

                var invoiceToSave = new Invoice
                {
                    Id = invoice.Id,
                    PdfFile = invoice.PdfFile,
                    PdfFileName = fileFileName,
                    PdfFileCreatedDate = datetime
                };

                invoiceRepository.UpdatePdf(invoiceToSave);
                invoiceRepository.Save();

                invoice.PdfFile = null;
                invoice.PdfFileName = fileFileName;
                invoice.PdfFileCreatedDate = datetime;

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.Billing.Invoice.PdfUpload, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> GetPdf(int invoiceId)
        {
            var response = new Response<Invoice>();

            var invoce = invoiceRepository.GetPdf(invoiceId);

            if (invoce == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            response.Data = invoce;
            return response;
        }

        public Response ChangeStatus(int invoiceId, InvoiceStatus status, EmailConfig emailConfig, InvoiceStatusParams parameters)
        {
            var response = new Response();

            var invoice = invoiceRepository.GetById(invoiceId);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceStatusHandler = invoiceStatusFactory.GetInstance(status);

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
                invoiceRepository.AddHistory(history);

                // Save
                invoiceRepository.Save();
                response.Messages.Add(new Message(invoiceStatusHandler.GetSuccessMessage(), MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                HandleSendMail(emailConfig, invoiceStatusHandler, invoice);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSendMail, MessageType.Error));
            }

            return response;
        }

        public IList<Invoice> GetOptions(string projectId)
        {
            return invoiceRepository.GetOptions(projectId);
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var invoice = invoiceRepository.GetById(id);

            if (invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            if (invoice.InvoiceStatus != InvoiceStatus.SendPending && invoice.InvoiceStatus != InvoiceStatus.Rejected)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.CannotDelete, MessageType.Error));
                return response;
            }

            try
            {
                invoiceRepository.Delete(invoice);
                invoiceRepository.Save();

                response.Messages.Add(new Message(Resources.Billing.Invoice.Deleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<Invoice> Search(InvoiceParams parameters, string userMail, EmailConfig emailConfig)
        {
            var isDirector = userRepository.HasDirectorGroup(userMail);
            var isDaf = userRepository.HasDafGroup(userMail, emailConfig.DafCode);
            var isCdg = userRepository.HasCdgGroup(userMail, emailConfig.CdgCode);

            if (isDirector || isDaf || isCdg)
            {
                return invoiceRepository.SearchByParams(parameters); 
            }
            else
            {
                return invoiceRepository.SearchByParamsAndUser(parameters, userMail);
            }
        }

        private void HandleSendMail(EmailConfig emailConfig, IInvoiceStatusHandler invoiceStatusHandler, Invoice invoice)
        {
            var subject = invoiceStatusHandler.GetSubjectMail(invoice);
            var body = invoiceStatusHandler.GetBodyMail(invoice, emailConfig.SiteUrl);
            var recipients = invoiceStatusHandler.GetRecipients(invoice, emailConfig);

            mailSender.Send(recipients, subject, body);
        }

        public Response<Invoice> Clone(int id)
        {
            var response = new Response<Invoice>();

            var invoice = invoiceRepository.GetSingle(x => x.Id == id);

            if(invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceToClone = invoice.Clone();

            try
            {
                invoiceRepository.Insert(invoiceToClone);
                invoiceRepository.Save();
                response.Data = invoiceToClone;
            }
            catch (Exception)
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<InvoiceHistory> GetHistories(int id)
        {
            return invoiceRepository.GetHistories(id);
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
