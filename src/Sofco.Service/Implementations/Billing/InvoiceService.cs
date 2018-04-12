using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Sofco.Common.Security.Interfaces;
using Sofco.Core.Config;
using Sofco.Core.DAL;
using Sofco.Core.Services.Billing;
using Sofco.Core.StatusHandlers;
using Sofco.Model.DTO;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;
using Sofco.Core.Mail;
using Sofco.Framework.ValidationHelpers.Billing;

namespace Sofco.Service.Implementations.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IInvoiceStatusFactory invoiceStatusFactory;
        private readonly IMailSender mailSender;
        private readonly ISessionManager sessionManager;
        private readonly EmailConfig emailConfig;

        public InvoiceService(IUnitOfWork unitOfWork, 
            IInvoiceStatusFactory invoiceStatusFactory,
            IOptions<EmailConfig> emailOptions,
            IMailSender mailSender, ISessionManager sessionManager)
        {
            this.unitOfWork = unitOfWork;
            this.invoiceStatusFactory = invoiceStatusFactory;
            this.mailSender = mailSender;
            this.sessionManager = sessionManager;
            this.emailConfig = emailOptions.Value;
        }

        public IList<Invoice> GetByProject(string projectId)
        {
            return unitOfWork.InvoiceRepository.GetByProject(projectId);
        }

        public Response<Invoice> GetById(int id)
        {
            var response = new Response<Invoice>();

            var invoce = unitOfWork.InvoiceRepository.GetById(id);

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

            InvoiceValidationHelper.ValidateCuit(response, invoice);
            InvoiceValidationHelper.ValidateAddress(response, invoice);
            InvoiceValidationHelper.ValidateZipCode(response, invoice);
            InvoiceValidationHelper.ValidateCity(response, invoice);
            InvoiceValidationHelper.ValidateProvince(response, invoice);
            InvoiceValidationHelper.ValidateCountry(response, invoice);

            if (response.HasErrors()) return response;

            try
            {
                var userName = identityName.Split('@')[0];
                var user = unitOfWork.UserRepository.GetSingle(x => x.UserName == userName);

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
                unitOfWork.InvoiceRepository.Insert(invoice);

                // Save
                unitOfWork.Save();

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

                unitOfWork.InvoiceRepository.UpdateExcel(invoiceToSave);
                unitOfWork.Save();

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

            var invoce = unitOfWork.InvoiceRepository.GetExcel(invoiceId);

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

                unitOfWork.InvoiceRepository.UpdatePdf(invoiceToSave);
                unitOfWork.Save();

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

            var invoce = unitOfWork.InvoiceRepository.GetPdf(invoiceId);

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

            var invoice = unitOfWork.InvoiceRepository.GetById(invoiceId);

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
                unitOfWork.InvoiceRepository.AddHistory(history);

                // Save
                unitOfWork.Save();
                response.Messages.Add(new Message(invoiceStatusHandler.GetSuccessMessage(), MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            try
            {
                // Send Mail
                invoiceStatusHandler.SendMail(mailSender, invoice, emailConfig);
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSendMail, MessageType.Error));
            }

            return response;
        }

        public IList<Invoice> GetOptions(string projectId)
        {
            return unitOfWork.InvoiceRepository.GetOptions(projectId);
        }

        public Response Delete(int id)
        {
            var response = new Response();

            var invoice = unitOfWork.InvoiceRepository.GetById(id);

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
                unitOfWork.InvoiceRepository.Delete(invoice);
                unitOfWork.Save();

                response.Messages.Add(new Message(Resources.Billing.Invoice.Deleted, MessageType.Success));
            }
            catch
            {
                response.Messages.Add(new Message(Resources.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public ICollection<Invoice> Search(InvoiceParams parameters)
        {
            var userMail = sessionManager.GetUserEmail();
            var isDirector = unitOfWork.UserRepository.HasDirectorGroup(userMail);
            var isDaf = unitOfWork.UserRepository.HasDafGroup(userMail);
            var isCdg = unitOfWork.UserRepository.HasCdgGroup(userMail);
            var isComercial = unitOfWork.UserRepository.HasComercialGroup(emailConfig.ComercialCode, userMail);

            if (isDirector || isDaf || isCdg || isComercial)
            {
                return unitOfWork.InvoiceRepository.SearchByParams(parameters);
            }

            return unitOfWork.InvoiceRepository.SearchByParamsAndUser(parameters, userMail);
        }

        public Response<Invoice> Clone(int id)
        {
            var response = new Response<Invoice>();

            var invoice = unitOfWork.InvoiceRepository.GetSingle(x => x.Id == id);

            if(invoice == null)
            {
                response.Messages.Add(new Message(Resources.Billing.Invoice.NotFound, MessageType.Error));
                return response;
            }

            var invoiceToClone = invoice.Clone();

            try
            {
                unitOfWork.InvoiceRepository.Insert(invoiceToClone);
                unitOfWork.Save();
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
            return unitOfWork.InvoiceRepository.GetHistories(id);
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
