using System;
using System.Collections.Generic;
using System.Linq;
using Sofco.Core.DAL.Admin;
using Sofco.Core.DAL.Billing;
using Sofco.Core.Services.Billing;
using Sofco.Model.Enums;
using Sofco.Model.Models.Billing;
using Sofco.Model.Utils;

namespace Sofco.Service.Implementations.Billing
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserRepository _userRepository;

        public InvoiceService(IInvoiceRepository invoiceRepository, IUserRepository userRepository)
        {
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
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

                invoice.CreatedDate = DateTime.Now;
                invoice.InvoiceStatus = InvoiceStatus.SendPending;
                invoice.InvoiceNumber = "00-00000000-00000";
                invoice.UserId = user.Id;

                _invoiceRepository.Insert(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.InvoiceCreated, MessageType.Success));
            }
            catch (Exception e)
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
                    ExcelFileName = string.Concat(invoice.AccountName, "_", invoice.Project, "_", datetime.ToString("d"), ".xlsx"),
                    ExcelFileCreatedDate = datetime
                };

                _invoiceRepository.UpdateExcel(invoiceToSave);
                _invoiceRepository.Save(string.Empty);

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.ExcelUpload, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> SendToDaf(int invoiceId)
        {
            var response = new Response<Invoice>();

            try
            {
                var exist = _invoiceRepository.Exist(invoiceId);

                if (!exist)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                    return response;
                }

                var invoice = new Invoice {Id = invoiceId, InvoiceStatus = InvoiceStatus.Sent};
                _invoiceRepository.UpdateStatus(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.SentToDaf, MessageType.Success));
            }
            catch (Exception e)
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
                    PdfFileName = string.Concat(invoice.AccountName, "_", invoice.Project, "_", datetime.ToString("d"), ".pdf"),
                    PdfFileCreatedDate = datetime
                };

                _invoiceRepository.UpdatePdf(invoiceToSave);
                _invoiceRepository.Save(string.Empty);

                response.Data = invoice;
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.PdfUpload, MessageType.Success));
            }
            catch (Exception e)
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

        public Response<Invoice> Reject(int invoiceId)
        {
            var response = new Response<Invoice>();

            try
            {
                var exist = _invoiceRepository.Exist(invoiceId);

                if (!exist)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                    return response;
                }

                var invoice = new Invoice { Id = invoiceId, InvoiceStatus = InvoiceStatus.Rejected };
                _invoiceRepository.UpdateStatus(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.Reject, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> Approve(int invoiceId, string invoiceNumber)
        {
            var response = new Response<Invoice>();

            try
            {
                if (string.IsNullOrWhiteSpace(invoiceNumber))
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.InvoiceNumerRequired, MessageType.Error));
                    return response;
                }

                var exist = _invoiceRepository.Exist(invoiceId);

                if (!exist)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                    return response;
                }

                var invoice = new Invoice { Id = invoiceId, InvoiceStatus = InvoiceStatus.Approved, InvoiceNumber = invoiceNumber };
                _invoiceRepository.UpdateStatusAndApprove(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.Approved, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
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

            if (invoice.InvoiceStatus != InvoiceStatus.SendPending)
            {
                response.Messages.Add(new Message(Resources.es.Billing.Invoice.CannotDelete, MessageType.Error));
                return response;
            }

            try
            {
                _invoiceRepository.Delete(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.Deleted, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }

        public Response<Invoice> Annulment(int invoiceId)
        {
            var response = new Response<Invoice>();

            try
            {
                var exist = _invoiceRepository.Exist(invoiceId);

                if (!exist)
                {
                    response.Messages.Add(new Message(Resources.es.Billing.Invoice.NotFound, MessageType.Error));
                    return response;
                }

                var invoice = new Invoice { Id = invoiceId, InvoiceStatus = InvoiceStatus.Cancelled };
                _invoiceRepository.UpdateStatus(invoice);
                _invoiceRepository.Save(string.Empty);

                response.Messages.Add(new Message(Resources.es.Billing.Invoice.Cancelled, MessageType.Success));
            }
            catch (Exception e)
            {
                response.Messages.Add(new Message(Resources.es.Common.ErrorSave, MessageType.Error));
            }

            return response;
        }
    }
}
